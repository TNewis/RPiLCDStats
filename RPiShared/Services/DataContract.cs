using System;
using System.Net.Sockets;
using System.Text;

namespace RPiLCDShared.Services
{
    public class DataContract
    {
        public Socket workSocket = null;
        public const int BufferSize = 1024;
        public byte[] buffer = new byte[BufferSize];
        public StringBuilder stringBuilder = new StringBuilder();

        public void ClearBuffer()
        {
            Array.Clear(buffer, 0, BufferSize);
        }
    }
}
