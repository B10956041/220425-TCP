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
using System.Threading;
using System.Collections;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        TcpListener Server;//伺服端網路監聽器
        Socket Client; //給客戶用的連線物件
        Thread Th_svr; //伺服器監聽用執行續
        Thread Th_clt; //客戶用的通話執行續
        Hashtable HT = new Hashtable();

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            CheckForIllegalCrossThreadCalls = false;
            Th_svr = new Thread(ServerSub);
            Th_svr.IsBackground = true;
            Th_svr.Start();
            button1.Enabled = false;
        }

        //接受客戶連線要求的方法 針對每一個客戶建立一個連線及獨立執行續
        private void ServerSub()
        {
            IPEndPoint EP = new IPEndPoint(IPAddress.Parse(textBox1.Text), int.Parse(textBox2.Text));

            Server = new TcpListener(EP);//建立server端監聽器
            Server.Start(100);//最多連線數100人
            while (true)
            {
                Client = Server.AcceptSocket(); //建立客戶的連線物件
                Th_clt = new Thread(Listen); //建立客戶連線獨立的執行續
                Th_clt.IsBackground = true;
                Th_clt.Start();
            }
        }
       private void Listen()
        {
            Socket Sck = Client;
            Thread Th = Th_clt;
            while (true)
            {
                try
                {
                    byte[] B = new byte[1023];
                    int inLen = Sck.Receive(B);
                    string Msg = Encoding.Default.GetString(B, 0, inLen);

                    string Cmd = Msg.Substring(0, 1);
                    string Str = Msg.Substring(1);
                    switch(Cmd)
                    {
                        case "0":
                            HT.Add(Str, Sck);
                            listBox1.Items.Add(Str);
                            break;
                        case "9":
                            HT.Remove(Str);
                            listBox1.Items.Remove(Str);
                            Th.Abort();
                            break;
                    }
                }
                catch (Exception)
                {

                }
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.ExitThread();//關閉所有執行續
        }
    }
}
