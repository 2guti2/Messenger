using System;
using System.Net.Sockets;
using System.Text;

namespace Protocol
{
    public class Connection
    {
        private const int LengthByteSize = 4;
        private Socket Socket { get; set; }

        public Connection(Socket socket)
        {
            Socket = socket;
        }

        public void SendMessage(object[] message)
        {
            string serializedMessage = Serializer.Serialize(message);
            byte[] data = Encoding.ASCII.GetBytes(serializedMessage);
            SendDataLength(data);
            SendData(data, data.Length);
        }

        public string[][][] ReadMessage()
        {
            int dataLength = ReadDataLength();
            byte[] dataReceived = ReadData(dataLength);
            string message = Encoding.UTF8.GetString(dataReceived);
            return Serializer.DeSerialize(message);
        }
        
        public void SendRawData(byte[] data)
        {
            SendDataLength(data);
            SendData(data, data.Length);
        }

        public byte[] ReadRawData()
        {
            int dataLength = ReadDataLength();
            return ReadData(dataLength);
        }
        
        public void Close()
        {
            Socket.Close();
        }

        public bool IsAlive()
        {
            try
            {
                return !(Socket.Poll(1, SelectMode.SelectRead) && Socket.Available == 0);
            }
            catch (SocketException)
            {
                return false;
            }
        }

        protected int ReadDataLength()
        {
            byte[] dataLengthAsBytes = ReadData(LengthByteSize);
            return BitConverter.ToInt32(dataLengthAsBytes, 0);
        }

        protected byte[] ReadData(int dataLength)
        {
            var dataReceived = new byte[dataLength];
            var received = 0;
            while (received < dataLength)
            {
                received += Socket.Receive(dataReceived, dataLength - received, SocketFlags.None);
            }

            return dataReceived;
        }

        protected void SendDataLength(byte[] data)
        {
            var length = data.Length;
            var dataLength = BitConverter.GetBytes(length);

            SendData(dataLength, LengthByteSize);
        }

        protected void SendData(byte[] data, int dataLength)
        {
            var sent = 0;
            while (sent < dataLength)
            {
                sent += Socket.Send(data, sent, dataLength - sent, SocketFlags.None);
            }
        }
    }
}