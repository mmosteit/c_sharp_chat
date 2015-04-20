using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace Chat
{
    class UTF8Socket
    {
        private Socket sock;

        public UTF8Socket(String ip)
        {
            sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint address = new IPEndPoint(IPAddress.Parse(ip), 9001);
            sock.Connect(address);
        }

        public UTF8Socket(Socket newSock)
        {
            sock = newSock;
        }

        public void SendMessage(String message)
        {
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);
            byte[] sizeBytes;
            int length = IPAddress.HostToNetworkOrder(messageBytes.Length);
            sizeBytes = BitConverter.GetBytes(length);

            sock.Send(sizeBytes);
            sock.Send(messageBytes);

        }

        public String ReceiveMessage()
        {
            String Message;
            int messageSize;
            byte[] sizeBytes = new byte[4];
            byte[] messageBytes;
            int numBytes;

            numBytes = sock.Receive(sizeBytes);

            if (numBytes == 0){
                throw new Exception("UTF8Socket received 0 bytes");
            }
            messageSize = BitConverter.ToInt32(sizeBytes, 0);
            messageSize = IPAddress.NetworkToHostOrder(messageSize);

            Console.WriteLine("Sizebytes = " + messageSize);

            messageBytes = new byte[messageSize];
            numBytes = sock.Receive(messageBytes);

            if (numBytes == 0){
                throw new Exception("UTF8Socket received 0 bytes");
            }
            Message = Encoding.UTF8.GetString(messageBytes, 0, messageSize);
            return Message;
        }

        public void close()
        {
            sock.Shutdown(SocketShutdown.Both);
            sock.Close();
        }
    }
}
