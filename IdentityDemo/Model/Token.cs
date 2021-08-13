using System;
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
}
