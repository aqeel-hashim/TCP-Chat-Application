using System;
using System.Collections.Generic;
using System.Text;
using Domain.Model;
using Domain.Repository;

namespace Data.Repository
{
    public class MessageSender : IMessageSender
    {
        public void Send(User user, string message)
        {
            throw new NotImplementedException();
        }

        public void Broadcast(List<User> users, string message)
        {
            throw new NotImplementedException();
        }
    }
}
