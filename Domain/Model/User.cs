using System;

namespace Domain.Model
{
    public class User
    {
        private string _ipAddress;
        private string _name;

        public string IpAddress
        {
            get => _ipAddress;
            set => _ipAddress = value;
        }

        public string Name
        {
            get => _name;
            set => _name = value;
        }

        public User(string ipAddress, string name)
        {
            _ipAddress = ipAddress;
            _name = name;
        }

        protected bool Equals(User other)
        {
            return string.Equals(_ipAddress, other._ipAddress) && string.Equals(_name, other._name);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((User) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((_ipAddress != null ? _ipAddress.GetHashCode() : 0) * 397) ^ (_name != null ? _name.GetHashCode() : 0);
            }
        }

     
    }
}
