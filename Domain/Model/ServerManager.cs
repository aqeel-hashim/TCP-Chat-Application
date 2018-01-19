using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Interactor;
using Domain.Repository;
using Newtonsoft.Json;

namespace Domain.Model
{
    public class ServerManager : IConnectionListener, IMessageReceiver
    {
        private Server _server;
        private IConnectionManager _connectionManager;
        private IMessageSender _messageSender;
        private IMessageProcessor _messageProcessor;

        public Server Server
        {
            get => _server;
            set => _server = value;
        }

        public IConnectionManager ConnectionManager
        {
            get => _connectionManager;
            set => _connectionManager = value;
        }

        public IMessageSender MessageSender
        {
            get => _messageSender;
            set => _messageSender = value;
        }

        public IMessageProcessor MessageProcessor
        {
            get => _messageProcessor;
            set => _messageProcessor = value;
        }

        public void Start()
        {
            _connectionManager.Start();
        }

        public void Stop()
        {
            _connectionManager.Stop();
        }

        public ServerManager(Server server, IConnectionManager connectionManager, IMessageSender messageSender, IMessageProcessor messageProcessor)
        {
            _server = server;
            _connectionManager = connectionManager;
            _messageSender = messageSender;
            _messageProcessor = messageProcessor;
        }

        public void Accept(User user)
        {
            var messageConnect =
                new Message(new User(_server.IpAddress, "SERVER"), user, "CONNECTION_SUCCESS", Message.Type.Connect);

            _messageSender.Send(user, JsonConvert.SerializeObject(messageConnect));
            _messageProcessor.Print(messageConnect);
        }

        public void SendMessage(Message message)
        {
            if(message.MessageType == Message.Type.OneToMany)
                _messageSender.Broadcast(_server.Users, JsonConvert.SerializeObject(message));
            else
                _messageSender.Send(message.ToUser, JsonConvert.SerializeObject(message));
            _messageProcessor.Print(message);
        }

        public void StartStatusCheck()
        {
            Parallel.Invoke(async () =>
            {
                while (true)
                {
                    try
                    {
                        Parallel.ForEach(_server.Users, (user) =>
                        {
                            var status = _connectionManager.CheckStatus(user);
                            switch (status)
                            {
                                case Status.Connected:
                                    var messageConnect =
                                        new Message(new User(_server.IpAddress, "SERVER"), user, "PING", Message.Type.Connect);
                                    _messageProcessor.Print(messageConnect);

                                    _messageSender.Send(user, JsonConvert.SerializeObject(messageConnect));
                                    break;
                                case Status.Disconnected:
                                    var userFrom = user;
                                    _server.Users.Remove(userFrom);
                                    _connectionManager.Disconnect(userFrom);
                                    var userstringlist = (string.Join(",", _server.Users.Select(x => x.ToString()).ToArray()));
                                    var messageDisconnect =
                                        new Message(new User(_server.IpAddress, "SERVER"), null,
                                            userstringlist, Message.Type.Refresh);
                                    _messageProcessor.Print(new Message(null, null, userFrom.Name+"@"+userFrom.IpAddress+" Disconnected", Message.Type.Disconnect));
                                    _messageProcessor.UpdateUsers(_server.Users);
                                    _messageSender.Broadcast(_server.Users, JsonConvert.SerializeObject(messageDisconnect));
                                    _messageSender.Broadcast(_server.Users, JsonConvert.SerializeObject(new Message(new User(_server.IpAddress, "Admin"), null, userFrom.Name + "@" + userFrom.IpAddress + " Disconnected", Message.Type.OneToMany )));
                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException();
                            }
                        });
                    }
                    catch (AggregateException ex)
                    {
                        foreach (var exception in ex.InnerExceptions)
                        {
                            Console.WriteLine(exception.StackTrace);
                        }
                    }

                    // don't run again for at least 200 milliseconds
                    await Task.Delay(1000);
                }
            });
            
        }

        public void Received(string message)
        {
            var receivedMessage = JsonConvert.DeserializeObject<Message>(message);
            _messageProcessor.Print(receivedMessage);
            User userFrom;
            string userstringlist;
            switch (receivedMessage.MessageType)
            {
                case Message.Type.Connect:
                    userFrom = receivedMessage.FromUser;
                    _server.Users.Add(userFrom);
                    userstringlist = (string.Join(";", _server.Users.Select(x => x.ToString()).ToArray()));
                    var messageConnect =
                        new Message(new User(_server.IpAddress, "SERVER"), null, JsonConvert.SerializeObject(_server.Users), Message.Type.Refresh);
                    _messageProcessor.UpdateUsers(_server.Users);
                    _messageSender.Broadcast(_server.Users, JsonConvert.SerializeObject(messageConnect));
                    _messageSender.Broadcast(_server.Users, JsonConvert.SerializeObject(new Message(new User(_server.IpAddress, "Admin"), null, userFrom.Name + "@" + userFrom.IpAddress + " Connected", Message.Type.OneToMany)));
                    break;
                case Message.Type.Disconnect:
                     userFrom = receivedMessage.FromUser;
                    _server.Users.Remove(userFrom);
                    _connectionManager.Disconnect(userFrom);
                    userstringlist = (string.Join(";", _server.Users.Select(x => x.ToString()).ToArray()));
                    var messageDisconnect =
                        new Message(new User(_server.IpAddress, "SERVER"), null, JsonConvert.SerializeObject(_server.Users), Message.Type.Refresh);
                    _messageProcessor.UpdateUsers(_server.Users);
                    _messageSender.Broadcast(_server.Users, JsonConvert.SerializeObject(messageDisconnect));
                    _messageSender.Broadcast(_server.Users, JsonConvert.SerializeObject(new Message(new User(_server.IpAddress, "Admin"), null, userFrom.Name + "@" + userFrom.IpAddress + " Disconnected", Message.Type.OneToMany)));
                    break;
                case Message.Type.OneToOne:
                    _messageProcessor.Print(receivedMessage);
                    _messageSender.Send(receivedMessage.ToUser, JsonConvert.SerializeObject(receivedMessage));
                    break;
                case Message.Type.OneToMany:
                    _messageProcessor.Print(receivedMessage);
                    _messageSender.Broadcast(_server.Users, JsonConvert.SerializeObject(receivedMessage));
                    break;
                default:
                    break;
            }
        }
    }
}
