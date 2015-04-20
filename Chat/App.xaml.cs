using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Chat
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public void Application_Startup(object sender, StartupEventArgs e)
        {
            String ip_addr;
            String username;
            String chatroom;
            String[] newArgs;
            MainWindow mainWin;

            if (e.Args.Length == 0)
            {
                Console.WriteLine("usage:");
                Console.WriteLine("ChatNew.exe server <ip address> [room_names]");
                Console.WriteLine("ChatNew.exe client <ip address> <room_name> <username>");
                Application.Current.Shutdown();
            }
            else
            {
                newArgs = new String[e.Args.Length - 1];

                for (int i = 0; i < newArgs.Length; i++)
                {
                    newArgs[i] = e.Args[i + 1];
                }

                if (e.Args[0] == "client")
                {
                    if (e.Args.Length != 4)
                    {
                        Console.WriteLine("Error: Must supply <ip address> <chatroom> <username>");
                    }
                    else
                    {
                        ip_addr = e.Args[1];
                        chatroom = e.Args[2];
                        username = e.Args[3];

                        mainWin = new MainWindow(ip_addr, chatroom, username);
                        mainWin.Show();
                    }

                }

                else if (e.Args[0] == "server")
                {
                    MainServer server = new MainServer(newArgs);
                    server.run();
                }

                else
                {
                    Console.WriteLine("Invalid argument " + e.Args[0]);
                }
            }
        }
    }
}
