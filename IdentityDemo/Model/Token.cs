﻿using System;
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
        public string AppName { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
    }
}
