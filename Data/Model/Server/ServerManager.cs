using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Model;
using Domain.Repository;
using Newtonsoft.Json;

namespace Data.Model.Server
{
    public class ServerManager : IMessageSender, IConnectionManager
    {

        private Listner _listner;
        private List<UserEntity> _clients;
        private IMessageReceiver _messageReceiver;

        public Listner Listner
        {
            get => _listner;
            set => _listner = value;
        }

        public List<UserEntity> Clients
        {
            get => _clients;
            set => _clients = value;
        }

        public IMessageReceiver MessageReceiver
        {
            get => _messageReceiver;
            set => _messageReceiver = value;
        }

        public ServerManager(Listner listner, IMessageReceiver messageReceiver)
        {
            _listner = listner;
            _messageReceiver = messageReceiver;
            _listner.SocketAccepted += listner_SocketAccepted;
            _clients = new List<UserEntity>();
        }

        public void Start()
        {
            _listner.Start();
        }

        public void Stop()
        {
            _listner.Stop();
        }

        private void listner_SocketAccepted(UserEntity userEntity)
        {
            Parallel.Invoke(() =>
            {
                userEntity.Socket.Received += Client_Received;
                _clients.Add(userEntity);
            });
        }

        private void Client_Received(string message)
        {
            _messageReceiver.Received(message);
        }


        public void Send(User user, string message)
        {
            Parallel.ForEach(_clients, (client) =>
            {
                if (client.User.IpAddress.Equals(user.IpAddress))
                {
                    client.Socket.Send(message);
                }
            });
        }

        public void Broadcast(List<User> users, string message)
        {
            Parallel.ForEach(_clients, (client) =>
            {
                Parallel.ForEach(users, (user) =>
                {
                    if (client.User.IpAddress.Equals(user.IpAddress))
                    {
                        client.Socket.Send(message);
                    }
                });
            });
        }

        public void Disconnect(User user)
        {
            Parallel.ForEach(_clients, (client) =>
            {
                if (!client.User.IpAddress.Equals(user.IpAddress)) return;
                client.Socket.Close();
                _messageReceiver.Received(JsonFormatter.FormatMessage(new Message(user, null, "DISCONNECT", Message.Type.Disconnect)));
                _clients.Remove(client);
            });
        }

        public Status CheckStatus(User user)
        {
            var status = Status.Unknown;
            Parallel.ForEach(_clients, (client) =>
            {
                if (client.User.IpAddress.Equals(user.IpAddress))
                {
                    status = client.Socket.Socket == null ? Status.Disconnected : Status.Connected;
                }
            });
            
            return status == Status.Unknown ? Status.Disconnected : Status.Connected ;
        }
    }
}
