using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using System.Net.Sockets;

namespace Chat
{

    class MainServer
    {
        private String ipAddress;
        private Dictionary<String, RoomThread> RoomMap;
        private RoomThread room;
        private Thread RoomThread_Thread;

        public MainServer(String [] args){
            ipAddress = args[0]; // The IP address to bind on
            RoomMap = new Dictionary<String, RoomThread>();
            for (int i = 1; i < args.Length; i++)
            {
                if (RoomMap.ContainsKey(args[i]))
                {
                    Console.WriteLine("Skipping redundant room "+args[i]);
                }
                else
                {
                    Console.WriteLine("Creating room " + args[i]);
                    room = new RoomThread(args[i]);
                    RoomMap.Add(args[i], room);
                }
                RoomThread_Thread = new Thread(room.run);
                RoomThread_Thread.Start();

            }

        }

        public void run(){
            String UserName;
            String RoomName;
            ClientThread client;
            UTF8Socket sock;
            Socket connection;
            Socket server;
            Thread thread;

            server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint address = new IPEndPoint(IPAddress.Parse(ipAddress), 9001);
            server.Bind(address);
            server.Listen(10);

            while(true){    

                connection = server.Accept();
                sock = new UTF8Socket(connection);

                RoomName = sock.ReceiveMessage();
                Console.WriteLine("Roomname = " + RoomName);

                if (!RoomMap.ContainsKey(RoomName))
                {
                    sock.SendMessage("REJECTED");
                    sock.close();
                    continue;
                }
                else
                {
                    sock.SendMessage("ACCEPTED");
                }

               
                UserName = sock.ReceiveMessage();
                Console.WriteLine("UserName = " + UserName);

                // 
                if (room.HasName(UserName)){

                    sock.SendMessage("REJECTED");
                    sock.close();
                    continue;
                }
                else
                {
                    sock.SendMessage("ACCEPTED");
                    client = new ClientThread(room, sock, UserName);
                    room.AddClient(client);
                    thread = new Thread(client.run);
                    thread.Start();
                    room.AddMessage("server", "<" + UserName + " has entered the room>");  
                }
            }
        }
    }
}

