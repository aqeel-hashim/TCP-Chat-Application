using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Domain.Model;
using Domain.Repository;

namespace Data.Model
{
    public class ServerManager : IMessageSender, IConnectionManager
    {

        private readonly Listner _listner;
        private List<UserEntity> _clients;
        private IMessageReceiver _messageReceiver;
        public Listner Listner => _listner;

        public List<UserEntity> Clients
        {
            get => _clients;
            set => _clients = value;
        }

        public ServerManager(Listner listner)
        {
            _listner = listner;
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
                if (client.User.Equals(user))
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
                    if (client.User.Equals(user))
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
                if (!client.User.Equals(user)) return;
                client.Socket.Close();
                _messageReceiver.Received(JsonFormatter.Format(new Message(user, null, "DISCONNECT", Message.Type.Disconnect)));
                _clients.Remove(client);
            });
        }

        public Status CheckStatus(User user)
        {
            var status = Status.Unknown;
            Parallel.ForEach(_clients, (client) =>
            {
                if (client.User.Equals(user))
                {
                    status = client.Socket.Socket == null ? Status.Disconnected : Status.Connected;
                }
            });
            
            return status == Status.Unknown ? Status.Disconnected : Status.Connected ;
        }
    }
}
