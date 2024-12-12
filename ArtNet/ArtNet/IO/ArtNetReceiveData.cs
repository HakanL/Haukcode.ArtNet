namespace Haukcode.ArtNet.IO
{
    public class ArtNetReceiveData
    {
        public byte[] buffer = new byte[1500];
        public int bufferSize = 1500;
        public int DataLength = 0;

        public bool Valid
        {
            get { return DataLength > 12; }
        }

        public ushort OpCode
        {
            get
            {
                return (ushort)(this.buffer[8] + (this.buffer[9] << 8));
            }
        }

        public ArtNetReceiveData()
        {
        }
    }
}
