using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat
{
    class ClientThread
    {
        private UTF8Socket sock;
        private RoomThread room;
        public String name;

        public ClientThread(RoomThread newroom, UTF8Socket newSock, String newName){
            sock = newSock;
            room = newroom;
            name = newName;
        }

        public void run()
        {
            String message;

            while (true)
            {
                try{
                    message = sock.ReceiveMessage();
                    room.AddMessage(name, name + ": " + message);

                }
                catch(Exception e ){
                    Console.WriteLine("About to call room.RemoveClient for "+name);
                    room.RemoveClient(name);
                    return;
                }
            }
        }

        public void SendToClient(String message){
            sock.SendMessage(message);
        }
    }

}
