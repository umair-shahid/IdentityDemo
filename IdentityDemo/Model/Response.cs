using System;
using Microsoft.AspNetCore.Http;

namespace IdentityDemo.Model
{
    public class Response
    {
        public Response()
        {

        }
        public dynamic Data { get; set; }
        public string Message { get; set; }
        public bool Success { get; set; }
        public string Status { get; set; }
    }
}
