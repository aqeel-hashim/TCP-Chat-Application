using System;
using System.Collections.Generic;
using System.Text;
using Domain.Model;

namespace Domain.Repository
{
    public interface IServerRequest
    {
        void Send(string message);
        void Disconnect();
        void Connect(string ipAddress);

    }
}
