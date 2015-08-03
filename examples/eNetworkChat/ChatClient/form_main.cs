using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using eNetwork2;

namespace ChatClient
{
    public partial class form_main : Form
    {
        eClient client;

        public form_main()
        {
            InitializeComponent();
            client = new eClient("127.0.0.1", 666, 667);
            client.OnDataReceived += Client_OnDataReceived;
        }

        private void Client_OnDataReceived(byte[] buffer)
        {
            PacketReader pr = new PacketReader(buffer);
            Byte ID = pr.ReadByte();

            if(ID == 1)
            {
                int id = pr.ReadInt32();
                string message = pr.ReadString();
                string msg = id + " : " + message;
                listBox_messages.Items.Add(msg);
            }
        }

        private void form_main_Load(object sender, EventArgs e)
        {
            client.Connect();
        }

        private void textBox_message_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
                PacketWriter pw = new PacketWriter();
                pw.WriteByte(1);
                pw.WriteString(textBox_message.Text);
                textBox_message.Clear();
                client.Send(pw.ToArray());
            }
        }
    }
}
