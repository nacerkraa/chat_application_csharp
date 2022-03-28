using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Net;
using System.Net.Sockets;

namespace chatApp1
{
    public partial class Form1 : Form
    {
        Socket sck;
        EndPoint epLocale, epRemote;
        byte[] buffer;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            sck = new Socket(AddressFamily.InterNetwork,SocketType.Dgram,ProtocolType.Udp);
            sck.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            //get user IP
            textLocalP.Text = GetLocaleIp();
            textRemoteIP.Text = GetLocaleIp();
        }

            private string GetLocaleIp()
            {
                IPHostEntry host;
                host = Dns.GetHostEntry(Dns.GetHostName());
                foreach(IPAddress ip in host.AddressList){
                    if(ip.AddressFamily == AddressFamily.InterNetwork)
                        return ip.ToString();
                }

                return "127.0.0.1";
            }

            private void buttonConnect_Click(object sender, EventArgs e)
            {
                // Pinding Socket
                epLocale = new IPEndPoint(IPAddress.Parse(textLocalP.Text), Convert.ToInt32(textLocalePort.Text));
                sck.Bind(epLocale);

                // Connecting to socket
                epRemote = new IPEndPoint(IPAddress.Parse(textRemoteIP.Text), Convert.ToInt32(textRemotePort.Text));
                sck.Connect(epRemote);

                // Lesning To Spicific Port
                buffer = new byte[1500];
                sck.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref epRemote, new AsyncCallback(MessageCallBack), buffer);
            }

            private void MessageCallBack(IAsyncResult aResult)
            {
                try
                {
                    byte[] recieveData = new byte[1500];
                    recieveData = (byte[])aResult.AsyncState;
                    // Converting byte to String
                    ASCIIEncoding aEncoding = new ASCIIEncoding();
                    string recivedMessage = aEncoding.GetString(recieveData);

                    // Adding this message Into List Box
                    listMessage.Items.Add("Friend: " + recivedMessage);
                    buffer = new byte[1500];
                    sck.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref epRemote, new AsyncCallback(MessageCallBack), buffer);
                }
                catch (Exception ex) {
                    MessageBox.Show(ex.ToString());
                }
            }

            private void button2_Click(object sender, EventArgs e)
            {
                // Convert String message to byte[]
                ASCIIEncoding aAncoding = new ASCIIEncoding();

                byte[] sendingMessage = new byte[1500];
                sendingMessage = aAncoding.GetBytes(textMessage.Text);
                // sended the encoded message
                sck.Send(sendingMessage);
                // adding to the list box
                listMessage.Items.Add("Me: " + textMessage.Text);
                textMessage.Text = "";
            }
        }
    }

