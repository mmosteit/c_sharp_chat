using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Net.Sockets;

namespace Chat
{
    public class RoomNameException : Exception
    {
        public RoomNameException(String message)
            : base(message)
        {

        }
    }

    public class UserNameException : Exception
    {
        public UserNameException(String message)
            : base(message)
        {

        }
    }

    class ClientGuiConnection
    {
        private UTF8Socket sock;
        private MainWindow gui;
        private String ip;
        private String chatroom;
        private String username;

        public ClientGuiConnection(MainWindow newGui, String ip, String chatroom, String username)
        {
            gui = newGui;
            this.ip = ip;
            this.chatroom = chatroom;
            this.username = username;
         
        }

        public void run()
        {
            try{
                sock = new UTF8Socket(ip);
            }
            catch (SocketException e){
                gui.DisplayMessage("Error: could not connect to server. Please restart program.");
                return;
            }

            String response;

            sock.SendMessage(chatroom);
            response = sock.ReceiveMessage();
            if (response != "ACCEPTED")
            {
                Console.WriteLine("chatroom rejected.");
                throw new RoomNameException("Room " + chatroom + " does not exist. Please restart the program");
            }

            sock.SendMessage(username);
            response = sock.ReceiveMessage();

            if (response != "ACCEPTED")
            {
                Console.WriteLine("username rejected");
                throw new UserNameException("Username " + username +" is already taken. Please restart the program");
            }

            String message;

            Console.WriteLine("About to enter main loop");
            Console.WriteLine("Just entered main loop");

            // The main loop
            while (true)
            {
                try
                {
                    message = sock.ReceiveMessage();

                }
                catch (Exception e)
                {
                    gui.DisplayMessage("Connection to server has been broken. Please make sure server is running and restart this program.");
                    return;
                }
                Console.WriteLine("Just received message " + message);
                gui.DisplayMessage(message);
            }

        }

        public void SendMessage(String message)
        {
            sock.SendMessage(message);
        }

   
        public void close()
        {
            sock.close();
        }
    }
}
