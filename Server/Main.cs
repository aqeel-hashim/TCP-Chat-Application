using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Threading.Tasks;
using Domain.Interactor;
using Domain.Model;
using Message = Domain.Model.Message;

namespace Server
{
    public partial class Main : Form, IMessageProcessor
    {
        private readonly Listener listener;
        private ServerManager _manager;
        public List<Socket> clients = new List<Socket>();//stores all the clients into  a list
        private List<User> users;
        public void BroadcastData(string data) // send to all clients
        {
            Parallel.ForEach(clients, (socket) =>
            {

                try { socket.Send(Encoding.ASCII.GetBytes(data)); }
                catch (Exception) { }

            });
        }

        public Main()
        {
            InitializeComponent();
            var dataserver = new Data.Model.Server.ServerManager(new Data.Model.Listner(2014, null));
            _manager = new ServerManager(new Domain.Model.Server(new List<User>(), "" ), dataserver, dataserver, this );
            ((Data.Model.Server.ServerManager) _manager.MessageSender).Listner.ConnectionListener = _manager;
            ((Data.Model.Server.ServerManager) _manager.ConnectionManager).Listner.ConnectionListener = _manager;
            _manager.Start();
            
        }
        
        

        

        private PrivateChat pChat;

  
        private void Main_Load(object sender, EventArgs e)
        {
            listener.Start();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            listener.Stop();
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            if (txtInput.Text != string.Empty)
            {
                _manager.SendMessage(new Message(new User(_manager.Server.IpAddress, "Admin"), null, txtInput.Text, Message.Type.OneToMany ));
            }
        }

        private void disconnectClientToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var list = clientList.SelectedItems.Cast<ListViewItem>();
            Parallel.ForEach(list, (item) =>
            {
                var client = item.Tag as Client;
                client.Send("Disconnect|");

            });
        }

        private void chatWithClientToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var list = clientList.SelectedItems.Cast<ListViewItem>();
            Parallel.ForEach(list, (item) =>
            {
                var client = item.Tag as Client;
                client.Send("Disconnect|");
                pChat = new PrivateChat(this);
                Task task = new Task(() => { this.Invoke(() => { pChat.Show(); });
                });
                pChat.Task = task;
                task.Start();

            });
        }

        private void txtInput_KeyDown(object sender, KeyEventArgs e)
        {
            if  (e.KeyCode == Keys.Enter)
            {
                btnSend.PerformClick();
            }
        }

        private void txtReceive_TextChanged(object sender, EventArgs e)
        {
            txtReceive.SelectionStart = txtReceive.TextLength;
        }


        public void Print(Message message)
        {
            txtReceive.Text += "\n" + message.FromUser + " says: " + message.MessageText;
        }

        public void UpdateUsers(List<User> users)
        {
            this.users = users;
            clientList.Clear();
            foreach (var user in this.users)
            {
                var item = new ListViewItem(new string[] {user.IpAddress, user.Name, "CONNECTED"}) { Tag = user };
                clientList.Items.Add(item);
            }
        }

        public void Disconnect()
        {
            Close();
        }
    }
}
