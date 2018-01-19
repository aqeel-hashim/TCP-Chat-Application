using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using Domain.Model;

namespace Data.Model
{
    public class UserEntity
    {
        private User _user;
        private SocketManager _socket;

        public User User
        {
            get => _user;
            set => _user = value;
        }

        public SocketManager Socket
        {
            get => _socket;
            set => _socket = value;
        }

        public UserEntity(User user, SocketManager socket)
        {
            _user = user;
            _socket = socket;
        }
    }
}
