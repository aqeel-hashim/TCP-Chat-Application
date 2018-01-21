using System;
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
                Console.WriteLine("Listner Socket Accept IP: "+userEntity.User.IpAddress+" and name: "+userEntity.User.Name);
            });
        }

        private void Client_Received(string message, SocketManager socket)
        {
            Message msg = JsonFormatter.DeserializeMessage(message);
            Parallel.ForEach(_clients, (client) =>
            {

                if (client.Socket.Equals(socket))
                    client.User = msg.FromUser;
                Console.WriteLine("Message Server Socket: " + message);
                
            });
            _messageReceiver.Received(message);
        }


        public void Send(User user, string message)
        {
            Parallel.ForEach(_clients, (client) =>
            {
                Console.WriteLine("Users are equal when sending from server: " + (client.User.IpAddress.Equals(user.IpAddress) && client.User.Name.Equals(user.Name)));
                Console.WriteLine("Users compared to client: \n" + "\t\tIp: "+client.User.IpAddress + ","+(user.IpAddress) + "\n\t\tUser names: " + client.User.Name + "," + (user.Name));
                if (client.User.IpAddress.Equals(user.IpAddress) && client.User.Name.Equals(user.Name))
                {
                    client.Socket.Send(message);
                }
            });
        }

        public void Broadcast(string message)
        {
            
            Parallel.ForEach(_clients, (client) =>
            {
                Console.WriteLine("Message Server Socket: " + message);
                client.Socket.Send(message);
            });
        }

        public void Disconnect(User user)
        {
            Parallel.ForEach(_clients, (client) =>
            {
                if (!client.User.IpAddress.Equals(user.IpAddress) && client.User.Name.Equals(user.Name)) return;
                _messageReceiver.Received(JsonFormatter.FormatMessage(new Message(user, new User(), "DISCONNECT", Message.Type.Disconnect)));
                client.Socket.Close();
                _clients.Remove(client);
            });
        }

        public Status CheckStatus(User user)
        {
            var status = Status.Unknown;
            Parallel.ForEach(_clients, (client) =>
            {
                if (client.User.IpAddress.Equals(user.IpAddress) && client.User.Name.Equals(user.Name))
                {
                    status = client.Socket.Socket == null ? Status.Disconnected : Status.Connected;
                }
            });
            
            return status == Status.Unknown ? Status.Disconnected : Status.Connected ;
        }
    }
}
