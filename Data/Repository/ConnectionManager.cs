using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using Domain.Model;
using Domain.Repository;

namespace Data.Repository
{
    public class ConnectionManager : IConnectionManager
    {
        public void Disconnect(User user)
        {
            throw new NotImplementedException();
        }

        public Status CheckStatus(User user)
        {
            throw new NotImplementedException();
        }
    }
}
