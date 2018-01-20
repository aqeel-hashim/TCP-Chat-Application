using System;
using System.Collections.Generic;
using System.Json;
using System.Text;
using System.Web.Script.Serialization;
namespace Domain.Model
{
    public class JsonFormatter
    {
        public static string FormatMessage(Message message)
        {
            return new JavaScriptSerializer().Serialize(message);
        }

        public static string FormatUserList(List<User> users)
        {
            return new JavaScriptSerializer().Serialize(users);
        }

        public static Message DeserializeMessage(string message)
        {
            return new JavaScriptSerializer().Deserialize<Message>(message);
        }

        public static List<User> DeserializeUsers(string users)
        {
            return new JavaScriptSerializer().Deserialize<List<User>>(users);
        }
    }
}
