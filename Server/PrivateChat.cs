using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using System.Threading.Tasks;

namespace Server
{
    public partial class PrivateChat : Form
    {
        private readonly Main Main;
        private readonly ClientSettings client;
        private string nm, current;
        public string NickName { get { return nm; } set { this.nm = value;  } }
        public bool single = false;
        private Task _task;

        public Task Task {
            get { return _task; }
            set { _task = value; }
        }
        public PrivateChat(Main main)
        {
            InitializeComponent();
            Main = main;
        }

        public PrivateChat(ClientSettings client, string nick, string current) {
            InitializeComponent();
            this.client = client;
            this.nm = nick;
            this.current = current;
            Text = current+": Private Chat: "+nick;
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            if (txtInput.Text != string.Empty)
            {
                if (single)
                {
                    Parallel.Invoke(() => { client.Send("pChat|" + txtInput.Text + "|" + nm + "|" + current); });
                    txtReceive.Text += "Me: " + txtInput.Text + "\r\n";
                   
                }
                else
                {
                    if (this.client == null)
                    {
                        var list = Main.clientList.SelectedItems.Cast<ListViewItem>();
                        Parallel.ForEach(list, (item) =>
                            {
                                var client = item.Tag as Client;
                                client.Send("pMessage|" + txtInput.Text);
                                this.Invoke(() => { txtReceive.Text += "Server: " + txtInput.Text + "\r\n"; });
                            });
                    }
                    else
                    {
                        var client = this.client;
                        var input = txtInput.Text;
                        Parallel.Invoke(() => { client.Send("pMessage|" + nm + "|" + input);  });
                        txtReceive.Text += nm + " says: " + txtInput.Text + "\r\n";
                    }
                }
                txtInput.Text = string.Empty;
            }
        }

        private void txtReceive_TextChanged(object sender, EventArgs e)
        {
            txtReceive.SelectionStart = txtReceive.TextLength;
        }

        private void Form_Closing(object sender, FormClosingEventArgs e)
        {
            _task.Dispose();
        }

        private void txtInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnSend.PerformClick();
            }
        }

    }
}
