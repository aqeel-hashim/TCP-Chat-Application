using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using Domain.Model;
using Message = Domain.Model.Message;

namespace Client
{

    public partial class    PrivateChatUser : Form
    {

        private readonly PublicChatForm Main;
        private ClientManager client;
        private User _other;
        private string nickname;

        public User Other
        {
            get => _other;
            set => _other = value;
        }

        public PrivateChatUser(ClientManager main, User other)
        {
            InitializeComponent();
            this.client = main;
            _other = other;
            Text = "To " + other.Name + "@" + other.IpAddress;
        }

        private void txtReceive_TextChanged(object sender, EventArgs e)
        {
            txtReceive.SelectionStart = txtReceive.TextLength;
        }

        private void txtInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnSend.PerformClick();
            }
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            if (txtInput.Text == string.Empty) return;
            client.SendMessage(txtInput.Text, _other, Message.Type.OneToOne);
            txtInput.Text = string.Empty;
        }

        private void PrivateChatUser_OnLoad(object sender, EventArgs e)
        {
            txtReceive.Text = "Begin";
        }

        private void Form_Closing(object sender, FormClosingEventArgs e)
        {
            
        }
    }
}
