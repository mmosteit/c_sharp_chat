using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Chat
{
    class NameMessage
    {
        public String Name;
        public String Message;
        public NameMessage(String NewName, String NewMessage)
        {
            Name = NewName;
            Message = NewMessage;
        }
    }

    class RoomThread
    {
        private Dictionary<String, ClientThread> NameThreadMap;
        private Queue<NameMessage> NameMessageQueue;
        private String RoomName;

        public RoomThread(String newName)
        {

            NameMessageQueue = new Queue<NameMessage>();
            NameThreadMap = new Dictionary<String, ClientThread>();
            RoomName = newName;
        }

        public void run()
        {
            NameMessage CurrentNameMessage;
            String message;

            while (true)
            {
                Monitor.Enter(NameMessageQueue);

                while (NameMessageQueue.Count == 0)
                {
                    Monitor.Wait(NameMessageQueue);
                }

                while (NameMessageQueue.Count > 0)
                {
                    CurrentNameMessage = NameMessageQueue.Dequeue();

                    Monitor.Enter(NameThreadMap);
                    foreach (KeyValuePair<String, ClientThread> item in NameThreadMap)
                    {
                        if(CurrentNameMessage.Name != item.Value.name ){

                            try{
                                item.Value.SendToClient(CurrentNameMessage.Message);
                            }

                            // There is something wrong with the client connection.
                            // Do nothing. The removal of the client will be taken 
                            //care of via the removeclient method.
                            catch (Exception e){
                                Console.WriteLine("Could not send to client");
                            }
                        }
                    }
                    Monitor.Exit(NameThreadMap);
                }
                Monitor.Exit(NameMessageQueue);
            }

        }

        public bool HasName(String name)
        {
            bool result;
            Monitor.Enter(NameThreadMap);
            result = NameThreadMap.ContainsKey(name);
            Monitor.Exit(NameThreadMap);
            return result;
        }

        public void AddMessage(String name, String message)
        {
            Monitor.Enter(NameMessageQueue);
            NameMessageQueue.Enqueue(new NameMessage(name, message));
            Monitor.Pulse(NameMessageQueue);
            Monitor.Exit(NameMessageQueue);
        }

        public void AddClient(ClientThread user)
        {
            Monitor.Enter(NameThreadMap);
            NameThreadMap.Add(user.name, user);
            Monitor.Exit(NameThreadMap);
        }

        public void RemoveClient(String name)
        {
            Monitor.Enter(NameThreadMap);
            try{
                Console.WriteLine("About to remove. Count= " + NameThreadMap.Count);
                NameThreadMap.Remove(name);
                Console.WriteLine("Just removed.    Count=  " + NameThreadMap.Count);
            }
            finally{
                Monitor.Exit(NameThreadMap);
            }

            AddMessage("", "<" + name + " has left room " + RoomName + ">");
            Console.WriteLine("<" + name + " has left room " + RoomName + ">");
        }
    }
}
