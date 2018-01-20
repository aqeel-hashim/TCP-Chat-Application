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

            _messageSender.Send(user, JsonFormatter.FormatMessage(messageConnect));
            _messageProcessor.Print(messageConnect);
        }

        public void SendMessage(Message message)
        {
            if(message.MessageType == Message.Type.OneToMany)
                _messageSender.Broadcast(_server.Users, JsonFormatter.FormatMessage(message));
            else
                _messageSender.Send(message.ToUser, JsonFormatter.FormatMessage(message));
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

                                    _messageSender.Send(user, JsonFormatter.FormatMessage(messageConnect));
                                    break;
                                case Status.Disconnected:
                                    var userFrom = user;
                                    _server.Users.Remove(userFrom);
                                    _connectionManager.Disconnect(userFrom);
                                    var userstringlist = (string.Join(",", _server.Users.Select(x => x.ToString()).ToArray()));
                                    var messageDisconnect =
                                        new Message(new User(_server.IpAddress, "SERVER"), new User(), 
                                            JsonFormatter.FormatUserList(_server.Users), Message.Type.Refresh);
                                    _messageProcessor.Print(new Message(new User(), new User(), userFrom.Name+"@"+userFrom.IpAddress+" Disconnected", Message.Type.Disconnect));
                                    _messageProcessor.UpdateUsers(_server.Users);
                                    _messageSender.Broadcast(_server.Users, JsonFormatter.FormatMessage(messageDisconnect));
                                    _messageSender.Broadcast(_server.Users, JsonFormatter.FormatMessage(new Message(new User(_server.IpAddress, "Admin"), new User(), userFrom.Name + "@" + userFrom.IpAddress + " Disconnected", Message.Type.OneToMany )));
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
            var receivedMessage = JsonFormatter.DeserializeMessage(message);
            _messageProcessor.Print(receivedMessage);
            User userFrom;
            switch (receivedMessage.MessageType)
            {
                case Message.Type.Connect:
                    userFrom = receivedMessage.FromUser;
                    _server.Users.Add(userFrom);
                    var messageConnect =
                        new Message(new User(_server.IpAddress, "SERVER"), null, JsonFormatter.FormatUserList(_server.Users), Message.Type.Refresh);
                    _messageProcessor.UpdateUsers(_server.Users);
                    _messageSender.Broadcast(_server.Users, JsonFormatter.FormatMessage(messageConnect));
                    _messageSender.Broadcast(_server.Users, JsonFormatter.FormatMessage(new Message(new User(_server.IpAddress, "Admin"), new User(), userFrom.Name + "@" + userFrom.IpAddress + " Connected", Message.Type.OneToMany)));
                    break;
                case Message.Type.Disconnect:
                     userFrom = receivedMessage.FromUser;
                    _server.Users.Remove(userFrom);
                    _connectionManager.Disconnect(userFrom);
                    var messageDisconnect =
                        new Message(new User(_server.IpAddress, "SERVER"), null, JsonFormatter.FormatUserList(_server.Users), Message.Type.Refresh);
                    _messageProcessor.UpdateUsers(_server.Users);
                    _messageSender.Broadcast(_server.Users, JsonFormatter.FormatMessage(messageDisconnect));
                    _messageSender.Broadcast(_server.Users, JsonFormatter.FormatMessage(new Message(new User(_server.IpAddress, "Admin"), new User(), userFrom.Name + "@" + userFrom.IpAddress + " Disconnected", Message.Type.OneToMany)));
                    break;
                case Message.Type.OneToOne:
                    _messageProcessor.Print(receivedMessage);
                    _messageSender.Send(receivedMessage.ToUser, JsonFormatter.FormatMessage(receivedMessage));
                    break;
                case Message.Type.OneToMany:
                    _messageProcessor.Print(receivedMessage);
                    _messageSender.Broadcast(_server.Users, JsonFormatter.FormatMessage(receivedMessage));
                    break;
                default:
                    break;
            }
        }
    }
}
