using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Threading;
using System.Threading.Tasks;
using Domain.Interactor;
using Domain.Model;
using Message = Domain.Model.Message;

namespace Client
{
    public partial class PublicChatForm : Form, IMessageProcessor
    {
        public PublicChatForm()
        {
            InitializeComponent();
            chats = new List<User>();
        }

        public readonly LoginForm formLogin = new LoginForm();

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            formLogin.Client.MessageProcessor =  this;
            formLogin.ShowDialog();
            Text = "TCP Chat - " + formLogin.txtIP.Text + " - (Connected as: " + formLogin.txtNickname.Text + ")";
        }
        
        private List<PrivateChatUser> privateChat;
        private List<User> chats;
       



        private void txtInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnSend.PerformClick();
            }
        }


        private void txtReceive_TextChanged(object sender, EventArgs e)
        {
            txtReceive.SelectionStart = txtReceive.TextLength;
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            var client = formLogin.Client;
            client.SendMessage(txtInput.Text, null, Message.Type.OneToMany);
            txtReceive.Text += "\n" + formLogin.txtNickname.Text + "says: " + txtReceive.Text + "\r\n";
            txtInput.Text = string.Empty;
        }

        private void privateChatToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (userList.SelectedItems[0] == null) return;
            var selectedItemText = ((ListViewItem)userList.SelectedItems[0]).Tag as User;
            var found = false;
            foreach (var chat in privateChat)
            {
                if (!chat.Other.Equals(selectedItemText)) continue;
                found = true;
            }

            if (found) return;
            {
                var chat = new PrivateChatUser(formLogin.Client, selectedItemText);
                chat.Show();
                privateChat.Add(chat);
            }


        }

        private void userList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (userList.SelectedItems[0] == null) return;
            var selectedItemText = ((ListViewItem) userList.SelectedItems[0]).Tag as User;
            var found = false;
            foreach (var chat in privateChat)
            {
                if (!chat.Other.Equals(selectedItemText)) continue;
                found = true;
            }

            if (found) return;
            {
                var chat = new PrivateChatUser(formLogin.Client, selectedItemText);
                chat.Show();
                privateChat.Add(chat);
            }
        }

        public void Print(Message message)
        {
            switch (message.MessageType)
            {
                case Message.Type.OneToMany:
                    txtReceive.Text += "\n" + message.FromUser + " says: " + message.MessageText;
                    break;
                case Message.Type.OneToOne:
                    var found = false;
                    foreach (var chat in privateChat)
                    {
                        if (!chat.Other.Equals(message.FromUser)) continue;
                        chat.txtReceive.Text += "\n" + message.FromUser + " says: " + message.MessageText;
                        found = true;
                    }

                    if (!found)
                    {
                        foreach (var user in chats)
                        {
                            if(!user.Equals(message.FromUser)) continue;
                            found = true;
                        }
                        if(!found)
                            chats.Add(message.FromUser);
                        var chat = new PrivateChatUser(formLogin.Client, message.FromUser);
                        chat.txtReceive.Text += "\n" + message.FromUser + " says: " + message.MessageText;
                        chat.Show();
                        privateChat.Add(chat);
                    }

                    break;
            }
        }

        public void UpdateUsers(List<User> users)
        {
            this.chats = users;
            foreach (var user in chats)
            {
                var item = new ListViewItem(user.Name) {Tag = user};
                this.userList.Items.Add(item);
            }
            
        }

        public void Disconnect()
        {
            Close();
        }
    }
}





