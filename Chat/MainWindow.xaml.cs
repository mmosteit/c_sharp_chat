using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Net.Sockets;
using System.Threading;

namespace Chat
{
    public delegate void DisplayMessageDelegate();
    public delegate void SendMessageDelegate();

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public partial class MainWindow : Window
    {
        private ClientGuiConnection connection;
        private Thread clientThread;
        public MainWindow(String ip, String chatroom, String username)
        {

            InitializeComponent();
            
            connection = new ClientGuiConnection(this, ip, chatroom, username);
            clientThread = new Thread(connection.run);
            clientThread.Start();

        }

        public void SetTitle(String newTitle)
        {
            this.Title = newTitle;
        }


        public void DisplayMessage(String text)
        {
            DisplayMessageClass Display;
            DisplayMessageDelegate del;
            if(!readText.Dispatcher.CheckAccess()){

                Display = new DisplayMessageClass(this, text);
                del     = Display.display;

                Dispatcher.BeginInvoke(DispatcherPriority.Normal, del);
            }
            else{

                readText.Text += "\r\n"+text;
            }
        }

        private void SendEvent(object sender, RoutedEventArgs e)
        {
            ProcessSend();
        }

        private void ProcessSend()
        {
            String messageText;
            SendMessageClass send;
            SendMessageDelegate del;

            messageText = writeText.Text;

            if (messageText.Trim() != "")
            {
                writeText.Text = "";

                DisplayMessage("Me: " + messageText);
            }

            send = new SendMessageClass(connection, messageText);
            del = send.send;
            this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, del);
        }

        private void KeyDownHandler(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return){
                ProcessSend();
            }       
        }

    }
    public class DisplayMessageClass
    {
        private String message;
        MainWindow win;
        public DisplayMessageClass(MainWindow window, String text)
        {
            message = text;
            win = window;
        }

        public void display()
        {
            win.DisplayMessage(message);
        }

    }

    class SendMessageClass
    {
        private String message;
        private ClientGuiConnection client;

        public SendMessageClass(ClientGuiConnection client, String message)
        {
            this.message = message;
            this.client  = client;
        }

        public void send()
        {
            client.SendMessage(message);
        }
    }
}
