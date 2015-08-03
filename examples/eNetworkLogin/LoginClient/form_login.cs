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
                bool success = pr.ReadBoolean();

                if(success)
                {
                    MessageBox.Show("Connection successful !");
                }
                else
                {
                    MessageBox.Show("Wrong password or username !");
                }

            }
        }
    }
}
