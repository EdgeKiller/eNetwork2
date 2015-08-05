using eNetwork2;
using System;
using System.Net.Sockets;
using System.Windows.Forms;

namespace LoginClient
{
    public partial class form_login : Form
    {

        private eClient client;

        public form_login()
        {
            InitializeComponent();
            client = new eClient("127.0.0.1", 666, 667);
        }

        private void form_login_Load(object sender, EventArgs e)
        {
            client.Connect();
        }

        private void button_connect_Click(object sender, EventArgs e)
        {
            if (textBox_username.Text.Trim() != "" && textBox_password.Text.Trim() != "")
            {
                PacketWriter pw = new PacketWriter();
                pw.WriteByte(1);
                pw.WriteString(textBox_username.Text.Trim());
                pw.WriteString(textBox_password.Text.Trim());

                byte[] response = client.SendRequest(pw.ToArray());

                PacketReader pr = new PacketReader(response);
                byte r = pr.ReadByte();

                if(r == 0)
                {
                    MessageBox.Show("Connection successful !");
                }
                else if(r == 2)
                {
                    MessageBox.Show("Banned : " + pr.ReadString());
                }
                else
                {
                    MessageBox.Show("Wrong password or username !");
                }

            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            client.Disconnect();
        }
    }
}
