using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Model
{
    public class Server
    {
        private List<User> _users;
        private string _ipAddress;

        public List<User> Users
        {
            get => _users;
            set => _users = value;
        }

        public string IpAddress
        {
            get => _ipAddress;
            set => _ipAddress = value;
        }

        public Server(List<User> users, string ipAddress)
        {
            _users = users;
            _ipAddress = ipAddress;
        }
    }
}
