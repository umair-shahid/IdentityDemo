using System.Threading.Tasks;
using IdentityDemo.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace IdentityDemo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class VerificationController : ControllerBase
    {
        [HttpGet]
        [AllowAnonymous]
        public Response Get()
        {
            return new Response() { Success = true, Message = "Verification controller", Data = null };
        }
        [HttpPost("Verify")]
        public async Task<IActionResult> Verify()
        {

                return Ok(new Response { Status = "Success", Message = "Verify successfully!" });
         
        }
    }
}
