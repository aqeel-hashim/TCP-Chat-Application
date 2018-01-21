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
            privateChat = new List<PrivateChatUser>();
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
            client.SendMessage(txtInput.Text, new User(), Message.Type.OneToMany);
            txtInput.Text = string.Empty;
        }

        private void privateChatToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (userList.SelectedItems[0] == null) return;
            var selectedItemText = ((ListViewItem)userList.SelectedItems[0]).Tag as User;
            var found = false;
            foreach (var chat in privateChat)
            {
                if (!chat.Other.IpAddress.Equals(selectedItemText?.IpAddress)) continue;
                chat.Show();
                formLogin.Client.SendMessage(formLogin.Client.CurrentUser.Name + "@" + formLogin.Client.CurrentUser.IpAddress + " wants to connect", selectedItemText, Message.Type.PrivateChatConnect);
                found = true;
            }

            if (found) return;
            var chatNew = new PrivateChatUser(formLogin.Client, selectedItemText);
            formLogin.Client.SendMessage(formLogin.Client.CurrentUser.Name + "@" + formLogin.Client.CurrentUser.IpAddress + " wants to connect", selectedItemText, Message.Type.PrivateChatConnect);
            chatNew.Show();
            privateChat.Add(chatNew);
            
        }

        private void userList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (userList.SelectedItems.Count <= 0 && userList.SelectedItems[0] == null) return;
            var selectedItemText = ((ListViewItem) userList.SelectedItems[0]).Tag as User;
            var found = false;
            foreach (var chat in privateChat)
            {
                if (!chat.Other.IpAddress.Equals(selectedItemText?.IpAddress)) continue;
                chat.Show();
                formLogin.Client.SendMessage(formLogin.Client.CurrentUser.Name + "@" + formLogin.Client.CurrentUser.IpAddress + " wants to connect", selectedItemText, Message.Type.PrivateChatConnect);
                found = true;
            }

            if (found) return;
            var chatNew = new PrivateChatUser(formLogin.Client, selectedItemText);
            formLogin.Client.SendMessage(formLogin.Client.CurrentUser.Name + "@" + formLogin.Client.CurrentUser.IpAddress + " wants to connect", selectedItemText, Message.Type.PrivateChatConnect);
            chatNew.Show();
            privateChat.Add(chatNew);
           
        }

        public void Print(Message message)
        {
            bool found = false;
            this.Invoke(() =>
            {
                Console.WriteLine("Message: "+message.MessageText);
                switch (message.MessageType)
                {
                    case Message.Type.OneToMany:
                        txtReceive.AppendText("\r\n" + message.FromUser.Name + " says: " + message.MessageText);
                        txtReceive.ScrollToCaret();
                        break;
                    case Message.Type.OneToOne:
                        foreach (var chat in privateChat)
                        {
                            if (!chat.Other.IpAddress.Equals(message.FromUser.IpAddress)) continue;
                            chat.txtReceive.AppendText("\r\n"+message.FromUser.Name + " says: " + message.MessageText);
                            chat.txtReceive.ScrollToCaret();
                            found = true;
                        }

                        if (!found)
                        {
                            foreach (var user in chats)
                            {
                                if (!user.IpAddress.Equals(message.FromUser.IpAddress)) continue;
                                found = true;
                            }
                            if (!found)
                                chats.Add(message.FromUser);
                            var chat = new PrivateChatUser(formLogin.Client, message.FromUser);
                            chat.txtReceive.AppendText("\r\n" + message.FromUser.Name + " says: " + message.MessageText);
                            chat.txtReceive.ScrollToCaret();
                            privateChat.Add(chat);
                        }

                        break;
                    case Message.Type.PrivateChatConnect:
                        found = false;
                        foreach (var chat in privateChat)
                        {
                            if (!chat.Other.IpAddress.Equals(message.FromUser.IpAddress)) continue;
                            chat.Show();
                            found = true;
                        }

                        if (found) break;
                        var chatNew = new PrivateChatUser(formLogin.Client, message.FromUser);
                        chatNew.Show();
                        privateChat.Add(chatNew);

                        break;
                    case Message.Type.PrivateChatDisconnect:
                        found = false;
                        foreach (var chat in privateChat)
                        {
                            if (!chat.Other.IpAddress.Equals(message.FromUser.IpAddress)) continue;
                            chat.Hide();
                        }
                        break;
                }
            });
        }

        public void UpdateUsers(List<User> users)
        {
            this.Invoke(() =>
            {
                this.userList.Items.Clear();
                this.chats = users;
                foreach (var user in chats)
                {
                    if (user.Equals(formLogin.Client.CurrentUser) || (user.Name.Equals(formLogin.Client.CurrentUser.Name) && user.IpAddress.Equals(formLogin.Client.CurrentUser.IpAddress))) continue;
                    var item = new ListViewItem(new[]{user.Name}) { Tag = user };
                    this.userList.Items.Add(item);
                }
            });
           
            
        }

        public void Disconnect()
        {
            this.Invoke(Close);
        }
    }
}





