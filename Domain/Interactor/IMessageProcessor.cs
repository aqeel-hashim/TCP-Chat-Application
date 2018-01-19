using System;
using System.Collections.Generic;
using System.Text;
using Domain.Model;

namespace Domain.Interactor
{
    public interface IMessageProcessor
    {
        void Print(Message message);
        void UpdateUsers(List<User> users);
        void Disconnect();
    }
}
