using System;
using System.Collections.Generic;
using System.Text;
using Domain.Interactor;
using Domain.Repository;
using Newtonsoft.Json;

namespace Domain.Model
{
    public class ClientManager : IMessageReceiver
    {
        private User _server;
        private User _currentUser;
        private List<User> _connectedUsers;
        private IServerRequest _serverRequest;
        private IMessageProcessor _messageProcessor;

        public User Server
        {
            get => _server;
            set => _server = value;
        }

        public User CurrentUser
        {
            get => _currentUser;
            set => _currentUser = value;
        }

        public List<User> ConnectedUsers
        {
            get => _connectedUsers;
            set => _connectedUsers = value;
        }

        public IMessageProcessor MessageProcessor
        {
            get => _messageProcessor;
            set => _messageProcessor = value;
        }

        public IServerRequest ServerRequest
        {
            get => _serverRequest;
            set => _serverRequest = value;
        }


        public ClientManager(User server, User currentUser, IMessageProcessor messageProcessor, IServerRequest serverRequest)
        {
            _server = server;
            _currentUser = currentUser;
            _connectedUsers = new List<User>();
            _messageProcessor = messageProcessor;
            _serverRequest = serverRequest;
        }

        public void Disconnect()
        {
            _serverRequest.Disconnect();
        }

        public void Connect(string nickname)
        {
            _serverRequest.Connect(_server.IpAddress, nickname);
        }

        public void SendMessage(string message, User user, Message.Type type)
        {
            var messageObj = new Message(_currentUser, user, message, type);
            _serverRequest.Send(JsonFormatter.FormatMessage(messageObj));
        }

        public void Received(string message)
        {
            var messageReceived = JsonFormatter.DeserializeMessage(message);
            switch (messageReceived.MessageType)
            {
                case Message.Type.OneToOne:
                case Message.Type.OneToMany:
                case Message.Type.PrivateChatConnect:
                case Message.Type.PrivateChatDisconnect:
                    _messageProcessor.Print(messageReceived);
                    break;
                case Message.Type.Connect:
                    if (messageReceived.MessageText.Equals("PING"))
                        break;
                    else if (messageReceived.MessageText.Equals("CONNECTION_SUCCESS"))
                    {
                        _serverRequest.Send(
                            JsonFormatter.FormatMessage(new Message(_currentUser, _server, "CONNECTION_SUCCESS", Message.Type.Connect)));
                        break;
                    }
                    break;
                case Message.Type.Disconnect:
                    Disconnect();
                    _messageProcessor.Disconnect();
                    break;
                case Message.Type.Refresh:

                    _messageProcessor.UpdateUsers(JsonFormatter.DeserializeUsers(messageReceived.MessageText));
                    break;
                default:
                    break;
            }
        }
    }
}
