using System;
using System.ComponentModel.DataAnnotations;

namespace IdentityDemo.Model
{
    public class ApplicationRegisteration
    {
        public ApplicationRegisteration()
        {
        }
        [Key]
        public int Id { get; set; }
        public string AppName { get; set; }
        public string SecretKey { get; set; }
        public string ClientId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ExpireDate { get; set; }
    }
}
