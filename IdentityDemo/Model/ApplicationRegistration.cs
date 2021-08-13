using System;

namespace IdentityDemo.Model
{
    public class ApplicationRegisteration
    {
        public ApplicationRegisteration()
        {
        }
        public string AppName { get; set; }
        public string SecretKey { get; set; }
        public string ClientId { get; set; }
        public DateTime CreatedAt { get; set; }

    }
}
