using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Model
{
    public class Message
    {
        private User _fromUser;
        private User _toUser;
        private string _message;
        private Type _type;
        public enum Type
        {
            OneToOne,
            OneToMany,
            Connect,
            Disconnect,
            Refresh
        }

        public User FromUser
        {
            get => _fromUser;
            set => _fromUser = value;
        }

        public User ToUser
        {
            get => _toUser;
            set => _toUser = value;
        }

        public string MessageText
        {
            get => _message;
            set => _message = value;
        }

        public Type MessageType
        {
            get => _type;
            set => _type = value;
        }

        public Message(User fromUser, User toUser, string message, Type type)
        {
            _fromUser = fromUser;
            _toUser = toUser;
            _message = message;
            _type = type;
        }

        protected bool Equals(Message other)
        {
            return Equals(_fromUser, other._fromUser) && Equals(_toUser, other._toUser) && string.Equals(_message, other._message) && _type == other._type;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Message) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (_fromUser != null ? _fromUser.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (_toUser != null ? _toUser.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (_message != null ? _message.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (int) _type;
                return hashCode;
            }
        }

       
    }
}
