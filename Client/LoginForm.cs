using System;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;
using Data.Model.Client;
using Domain;
using Domain.Model;
using ClientManager = Domain.Model.ClientManager;

namespace Client
{
    public partial class LoginForm : Form
    {
        public ClientManager Client { get; set; }
        public LoginForm()
        {
            string hostName = Dns.GetHostName(); // Retrive the Name of HOST  
            Console.WriteLine(hostName);
            // Get the IP  
            string myIP = Dns.GetHostEntry(hostName).AddressList[0].ToString();
            var current = new User(myIP, "");
            Client = new ClientManager(new User("","SERVER"), current, null, new Data.Model.Client.ClientManager(new UserEntity(current, new Data.Model.Client.SocketManager()), null ));
            ((Data.Model.Client.ClientManager) Client.ServerRequest).MessageReceiver = Client;
            InitializeComponent();
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            
            Parallel.Invoke(() =>
            {
                Client.Server.IpAddress = txtIP.Text;
                Client.CurrentUser.Name = txtNickname.Text;
                Client.Connect();
            });
            Close();

        }

    }
}
