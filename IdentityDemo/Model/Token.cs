using System;
using System.ComponentModel.DataAnnotations;

namespace IdentityDemo.Model
{
    public class Token
    {
        public Token()
        {
        }
        public object AccessToken { get; set; }
        public object Expire { get; set; }
    }
    public class TokenDetail
    {
        public object Jti { get; set; }
        public object NameIdentifier { get; set; }
        public object Sub { get; set; }
        public object CHash { get; set; }
    }
    public class TokenGeneration
    {
        public TokenGeneration()
        {
        }
        [Required(ErrorMessage ="App name is required")]
        public string AppName { get; set; }
        [Required(ErrorMessage = "Client id is required")]
        public string ClientId { get; set; }
        [Required(ErrorMessage = "Client secret is required")]
        public string ClientSecret { get; set; }
    }
}
