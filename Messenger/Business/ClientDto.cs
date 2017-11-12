using System.Runtime.Serialization;

namespace Business
{
    [DataContract]
    public class ClientDto
    {
        [DataMember]
        public string Username { get; set; }
        [DataMember]
        public string Password { get; set; }
    }
}