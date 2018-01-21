using System;
using System.Collections.Generic;
using System.Text;
using Domain.Model;

namespace Domain.Repository
{
    public interface IMessageSender
    {
        void Send(User user, string message);
        void Broadcast(string message);
    }
}
