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
            client = new eClient("127.0.0.1", 666);
        }

        private void form_main_Load(object sender, EventArgs e)
        {
            client.Connect();
        }

        private void textBox_message_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
                byte[] buffer;
                PacketWriter.WriteByte(ref buffer, 1);
            }
        }
    }
}
