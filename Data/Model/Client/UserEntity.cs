using System;
using System.Collections.Generic;
using System.Text;
using Domain.Model;

namespace Data.Model.Client
{
    public class UserEntity
    {
        private User _user;
        private SocketManager _socketManager;

        public User User
        {
            get => _user;
            set => _user = value;
        }

        public SocketManager SocketManager
        {
            get => _socketManager;
            set => _socketManager = value;
        }

        public UserEntity(User user, SocketManager socketManager)
        {
            _user = user;
            _socketManager = socketManager;
        }


    }
}
