using System;
using System.Collections.Generic;
using System.Text;
using Domain.Repository;

namespace Data.Repository
{
    public class ServerRequest : IServerRequest
    {
        public void Send(string message)
        {
            throw new NotImplementedException();
        }

        public void Disconnect()
        {
            throw new NotImplementedException();
        }

        public void Connect(string ipAddress)
        {
            throw new NotImplementedException();
        }
    }
}
