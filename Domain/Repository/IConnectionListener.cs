using System;
using System.Collections.Generic;
using System.Text;
using Domain.Model;

namespace Domain.Repository
{
    public interface IConnectionListener
    {
        void Accept(User user);

    }
}
