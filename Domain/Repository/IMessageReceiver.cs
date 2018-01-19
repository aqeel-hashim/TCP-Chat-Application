using System;
using System.Collections.Generic;
using System.Text;
using Domain.Model;

namespace Domain.Repository
{
    public interface IMessageReceiver
    {
        void Received(string message);

    }
}
