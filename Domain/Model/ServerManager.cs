﻿using System;
using System.Collections.Generic;
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

            _messageSender.Send(user, JsonFormatter.Format(messageConnect));
        }

        public void SendMessage(Message message)
        {
            message.FromUser = new User(_server.IpAddress, "SERVER");
            if(message.MessageType == Message.Type.OneToMany)
                _messageSender.Broadcast(_server.Users, JsonFormatter.Format(message));
            else
                _messageSender.Send(message.ToUser, JsonFormatter.Format(message));
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
                                    _messageSender.Send(user, JsonFormatter.Format(messageConnect));
                                    break;
                                case Status.Disconnected:
                                    var userFrom = user;
                                    _server.Users.Remove(userFrom);
                                    _connectionManager.Disconnect(userFrom);
                                    var messageDisconnect =
                                        new Message(new User(_server.IpAddress, "SERVER"), null,
                                            JsonConvert.SerializeObject(_server.Users), Message.Type.Connect);
                                    _messageProcessor.Print(messageDisconnect);
                                    _messageSender.Broadcast(_server.Users, JsonFormatter.Format(messageDisconnect));
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
                        new Message(new User(_server.IpAddress, "SERVER"), null, JsonConvert.SerializeObject(_server.Users), Message.Type.Refresh);

                    _messageSender.Broadcast(_server.Users, JsonFormatter.Format(messageConnect));
                    break;
                case Message.Type.Disconnect:
                     userFrom = receivedMessage.FromUser;
                    _server.Users.Remove(userFrom);
                    _connectionManager.Disconnect(userFrom);
                    var messageDisconnect =
                        new Message(new User(_server.IpAddress, "SERVER"), null, JsonConvert.SerializeObject(_server.Users), Message.Type.Refresh);

                    _messageSender.Broadcast(_server.Users, JsonFormatter.Format(messageDisconnect));
                    break;
                case Message.Type.OneToOne:
                    _messageSender.Send(receivedMessage.ToUser, JsonFormatter.Format(receivedMessage));
                    break;
                case Message.Type.OneToMany:
                    _messageSender.Broadcast(_server.Users, JsonFormatter.Format(receivedMessage));
                    break;
                default:
                    break;
            }
        }
    }
}