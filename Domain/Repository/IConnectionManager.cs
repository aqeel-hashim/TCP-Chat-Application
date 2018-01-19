using System;
using System.Collections.Generic;
using System.Text;
using Domain.Model;

namespace Domain.Repository
{
    public enum Status
    {
        Connected,
        Disconnected,
        Unknown
    }

    public interface IConnectionManager
    {
        void Start();
        void Stop();
        void Disconnect(User user);
        Domain.Repository.Status CheckStatus(User user);
    }
}
