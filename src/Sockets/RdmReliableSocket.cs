using Haukcode.Rdm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Sockets;
using System.Threading;

namespace Haukcode.Sockets
{
    /// <summary>
    /// This RDM socket provides a reliable means of transporting RDM packets over an unreliable network. It
    /// will wrap an unreliable RDM socket.
    /// </summary>
    /// <remarks>
    /// Ensures that a transaction is completed by re-requesting packets for which no response has been received.
    /// </remarks>
    public class RdmReliableSocket : IRdmSocket, INotifyPropertyChanged, IDisposable
    {
        private IRdmSocket socket = null;
        private Dictionary<int, Transaction> transactionQueue = new Dictionary<int, Transaction>();
        private Timer retryTimer;

        private class Transaction
        {
            public Transaction(int transactionId, RdmPacket packet, RdmEndPoint address, UId id)
            {
                Id = transactionId;
                TransactionNumber = (byte)(transactionId % 255);
                Packet = packet;
                TargetAddress = address;
                TargetId = id;
                Attempts = 0;
                LastAttempt = DateTime.MinValue;
            }

            public int Id;
            public byte TransactionNumber;
            public RdmPacket Packet;
            public RdmEndPoint TargetAddress;
            public UId TargetId;

            public int Attempts = 0;
            public DateTime LastAttempt = DateTime.MinValue;
        }

        private class TransactionUniverseComparer : IEqualityComparer<Transaction>
        {

            public bool Equals(Transaction x, Transaction y)
            {
                return x.TargetAddress.IpAddress == y.TargetAddress.IpAddress
                    && x.TargetAddress.Universe == x.TargetAddress.Universe;
            }

            public int GetHashCode(Transaction obj)
            {
                return obj.TargetAddress.IpAddress.GetHashCode() << 8 ^ obj.TargetAddress.Universe.GetHashCode();
            }
        }

        public RdmReliableSocket(IRdmSocket socket)
        {
            this.socket = socket;

            socket.NewRdmPacket += new EventHandler<NewPacketEventArgs<RdmPacket>>(socket_NewRdmPacket);
            socket.RdmPacketSent += new EventHandler<NewPacketEventArgs<RdmPacket>>(socket_RdmPacketSent);

            retryTimer = new Timer(new TimerCallback(Retry));
        }

        public IRdmSocket InnerSocket
        {
            get { return socket; }
        }

        private TimeSpan retryInterval = new TimeSpan(0, 0, 0, 20);

        public TimeSpan RetryInterval
        {
            get { return retryInterval; }
            set { retryInterval = value; }
        }

        private TimeSpan transmitInterval = new TimeSpan(0, 0, 0, 0, 40);

        public TimeSpan TransmitInterval
        {
            get { return transmitInterval; }
            set { transmitInterval = value; }
        }

        private int retryAttempts = 3;

        public int RetryAttempts
        {
            get { return retryAttempts; }
            set { retryAttempts = value; }
        }

        private int transactionNumber = 1;

        public int TransactionNumber
        {
            get { return transactionNumber; }
            protected set { transactionNumber = value; }
        }

        private int AllocateTransactionNumber()
        {
            lock (transactionQueue)
            {
                do
                {
                    if (TransactionNumber == int.MaxValue)
                        TransactionNumber = 1;
                    else
                        TransactionNumber++;
                }
                while (transactionQueue.ContainsKey(TransactionNumber));

                return TransactionNumber;
            }
        }

        private int packetsSent = 0;

        public int PacketsSent
        {
            get { return packetsSent; }
            protected set
            {
                if (packetsSent != value)
                {
                    packetsSent = value;
                    RaisePropertyChanged("PacketsSent");
                }
            }
        }

        private int packetsReceived = 0;

        public int PacketsReceived
        {
            get { return packetsReceived; }
            protected set
            {
                if (packetsReceived != value)
                {
                    packetsReceived = value;
                    RaisePropertyChanged("PacketsReceived");
                }
            }
        }

        private int packetsDropped = 0;

        public int PacketsDropped
        {
            get { return packetsDropped; }
            protected set
            {
                if (packetsDropped != value)
                {
                    packetsDropped = value;
                    RaisePropertyChanged("PacketsDropped");
                }
            }
        }

        private int transactionsStarted = 0;

        public int TransactionsStarted
        {
            get { return transactionsStarted; }
            protected set
            {
                if (transactionsStarted != value)
                {
                    transactionsStarted = value;
                    RaisePropertyChanged("TransactionsStarted");
                }
            }
        }

        private int transactionsFailed = 0;

        public int TransactionsFailed
        {
            get { return transactionsFailed; }
            protected set
            {
                if (transactionsFailed != value)
                {
                    transactionsFailed = value;
                    RaisePropertyChanged("TransactionsFailed");
                }
            }
        }

        private void RegisterTransaction(RdmPacket packet, RdmEndPoint address, UId id)
        {
            lock (transactionQueue)
            {
                if (packet.Header.Command == RdmCommands.Get || packet.Header.Command == RdmCommands.Set)
                {
                    int transactionId = AllocateTransactionNumber();
                    Transaction transaction = new Transaction(transactionId, packet, address, id);
                    transactionQueue.Add(transactionId, transaction);
                    packet.Header.TransactionNumber = transaction.TransactionNumber;

                    if (transactionQueue.Count == 1 && retryTimer != null)
                        retryTimer.Change(TransmitInterval, TimeSpan.Zero);

                    TransactionsStarted++;
                }
            }
        }

        /// <summary>
        /// Processes the transaction queue and determines what transactions can be sent to their destination.
        /// </summary>
        /// <remarks>
        /// This function ensures that only a single transaction is sent to each DMX port every 20ms. Any more 
        /// transactions per port and the device might become flooded.
        /// </remarks>
        /// <param name="state">Thread State</param>
        private void Retry(object state)
        {
            DateTime timeStamp = DateTime.Now;
            List<Transaction> failedTransactions = new List<Transaction>();
            HashSet<Transaction> retryTransactions = new HashSet<Transaction>(new TransactionUniverseComparer());
            int droppedPackets = 0;

            lock (transactionQueue)
            {
                //Go through all queued transactions and determine what can be sent again.
                foreach (Transaction transaction in transactionQueue.Values)
                {
                    //Only process this transaction if it has exceeded the retry interval.
                    if (transaction.LastAttempt == DateTime.MinValue || timeStamp.Subtract(transaction.LastAttempt) > RetryInterval)
                    {
                        //We only send one packet per retry to each unique DMX port.
                        //The retryTransactions hash set is used to ensure only one transaction is sent to each port. The rest have to wait.
                        if (transaction.Attempts > RetryAttempts)
                            failedTransactions.Add(transaction);
                        else if (!retryTransactions.Contains(transaction))
                        {
                            //If we have already tried to send this transaction then increment the dropped packet count.
                            if (transaction.Attempts != 0)
                                droppedPackets++;

                            //Queue this transaction for sending
                            retryTransactions.Add(transaction);
                            transaction.Attempts++;
                            transaction.LastAttempt = timeStamp;
                        }
                    }
                }

                //Remove all transactions that have perminantly failed.
                foreach (Transaction transaction in failedTransactions)
                    transactionQueue.Remove(transaction.Id);
            }

            PacketsDropped += droppedPackets + failedTransactions.Count;
            TransactionsFailed += failedTransactions.Count;

            foreach (Transaction transaction in retryTransactions)
            {
                try
                {
                    socket.SendRdm(transaction.Packet, transaction.TargetAddress, transaction.TargetId);
                }
                catch (SocketException)
                {
                    //If the connection has failed, remove the transaction from the queue to prevent further communications.
                    transactionQueue.Remove(transaction.Id);
                }
            }

            lock (transactionQueue)
                retryTimer.Change(TransmitInterval, TimeSpan.Zero);
        }

        void socket_RdmPacketSent(object sender, NewPacketEventArgs<RdmPacket> e)
        {
            PacketsSent++;

            if (RdmPacketSent != null)
                RdmPacketSent(sender, e);
        }

        void socket_NewRdmPacket(object sender, NewPacketEventArgs<RdmPacket> e)
        {
            PacketsReceived++;

            if (e.Packet.Header.TransactionNumber > 0)
            {
                if (e.Packet.Header.Command == RdmCommands.GetResponse || e.Packet.Header.Command == RdmCommands.SetResponse)
                {
                    lock (transactionQueue)
                    {
                        Transaction transaction = transactionQueue.Values.FirstOrDefault(item => item.TransactionNumber == e.Packet.Header.TransactionNumber);
                        if (transaction != null)
                            transactionQueue.Remove(transaction.Id);

                        if (transactionQueue.Count == 0)
                            retryTimer.Change(Timeout.Infinite, Timeout.Infinite);
                    }
                }
            }

            if (NewRdmPacket != null)
                NewRdmPacket(sender, e);
        }

        #region Events

        public event EventHandler<NewPacketEventArgs<Rdm.RdmPacket>> NewRdmPacket;
        public event EventHandler<NewPacketEventArgs<Rdm.RdmPacket>> RdmPacketSent;

        #endregion

        #region Communications

        public void SendRdm(RdmPacket packet, RdmEndPoint targetAddress, UId targetId)
        {
            //Queue this packet for sending.
            RegisterTransaction(packet, targetAddress, targetId);

            //socket.SendRdm(packet, targetAddress, targetId);
        }

        public void SendRdm(RdmPacket packet, RdmEndPoint targetAddress, UId targetId, UId sourceId)
        {
            //Queue this packet for sending.
            RegisterTransaction(packet, targetAddress, targetId);

            // socket.SendRdm(packet, targetAddress, targetId, sourceId);
        }

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        public void Dispose()
        {
            if (retryTimer != null)
            {
                retryTimer.Dispose();
                retryTimer = null;
            }
        }
    }
}
