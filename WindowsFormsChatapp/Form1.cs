using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsChatapp
{
    public partial class Form1 : Form
    {
        private TcpClient client;
        public StreamReader STR;
        public StreamWriter STW;
        public string recieve;
        public string TextToSend;

        public Form1()
        {
            InitializeComponent();

            IPAddress[] localIP = Dns.GetHostAddresses(Dns.GetHostName());

            foreach (IPAddress ip in localIP)
            {
                if(ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    ServerIPtextBox.Text = ip.ToString();
                }
            }
        }

        private void Startbutton_Click(object sender, EventArgs e)
        {
            TcpListener listener = new TcpListener(IPAddress.Any ,int.Parse(ServerPorttextBox.Text));

            listener.Start();

            client = listener.AcceptTcpClient();

            STR = new StreamReader(client.GetStream());

            STW =new StreamWriter(client.GetStream());

            STW.AutoFlush = true;

            backgroundWorker1.RunWorkerAsync();

            backgroundWorker2.WorkerSupportsCancellation = true;
        }

        private void Connectbutton_Click(object sender, EventArgs e)
        {
            client =new TcpClient();

            IPEndPoint iPEnd =new IPEndPoint(IPAddress.Parse(ClientIPtextBox.Text), int.Parse(ClientPorttextBox.Text));
            
            try
            {
                ChatScreenrichTextBox.AppendText("Connect to Server" + "\n");

                STR = new StreamReader(client.GetStream());

                STW = new StreamWriter(client.GetStream());

                STW.AutoFlush = true;

                backgroundWorker1.RunWorkerAsync();

                backgroundWorker2.WorkerSupportsCancellation = true;


            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            while (client.Connected)
            {
                try {
                recieve =STR.ReadLine();
                    this.ChatScreenrichTextBox.Invoke(new MethodInvoker(delegate()
                    {
                        ChatScreenrichTextBox.AppendText("you: " + recieve + "\n");
                    }));
                    recieve = "";
                
                
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }
            }
        }

        private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
        {
            if (client.Connected)
            {
                STW.WriteLine(TextToSend);
                this.ChatScreenrichTextBox.Invoke(new MethodInvoker(delegate()
                {
                    ChatScreenrichTextBox.AppendText("me: " + TextToSend + "\n");
                }));
            }
            else
            {
                MessageBox.Show("Sending failed");
            }
            backgroundWorker2.CancelAsync();
        }

        private void Sendbutton_Click(object sender, EventArgs e)
        {
            if (MessagerichTextBox.Text != "")
            {
                TextToSend = MessagerichTextBox.Text;
                backgroundWorker2.RunWorkerAsync();
            }
            MessagerichTextBox.Text = "";
        }
    }
}
