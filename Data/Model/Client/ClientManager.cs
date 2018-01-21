using System;
using System.Collections.Generic;
using System.Text;
using Domain.Repository;

namespace Data.Model.Client
{
    public class ClientManager : IServerRequest
    {
        private UserEntity _userEntity;
        private IMessageReceiver _messageReceiver;

        public IMessageReceiver MessageReceiver
        {
            get => _messageReceiver;
            set => _messageReceiver = value;
        }

        public ClientManager(UserEntity userEntity, IMessageReceiver messageReceiver)
        {
            _userEntity = userEntity;
            _messageReceiver = messageReceiver;
            _userEntity.SocketManager.Received += Client_Received;
        }

        private void Client_Received(string message)
        {
            _messageReceiver.Received(message);
        }

        public void Send(string message)
        {
            _userEntity.SocketManager.Send(message);
        }

        public void Disconnect()
        {
            _userEntity.SocketManager.Close();
        }

        public void Connect(string ipAddress, string nickname)
        {
            _userEntity.SocketManager.Connect(ipAddress, 2014, nickname);
        }
    }
}
