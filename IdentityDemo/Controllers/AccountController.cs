using IdentityDemo.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;
using IdentityDemo.Service.Interface;

namespace IdentityDemo.Controllers
{
    [ApiController]
    [Route("[controller]")]

    public class AccountController: ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly ISecurity _security;
        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, IConfiguration config, ISecurity security)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = config;
            _security = security;
        }

        [HttpGet]
        public Response Get()
        {
            return new Response() { Success = true, Message = "Api is running", Data = null };
        }

        [HttpPost("RegisterUser")]
        public async Task<IActionResult> RegisterUser(Register model)
        { 

              var userExists = await _userManager.FindByNameAsync(model.UserName);
                if (userExists != null)
                    return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User already exists!" });

                IdentityUser user = new IdentityUser()
                {
                    Email = model.Email,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    UserName = model.UserName
                };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    return Ok(new Response { Status = "Success", Message = "User created successfully!" });
                }
            return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User creation failed! Please check user details and try again." });
      
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(Login model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
            {
                Token token = _security.GenerateToken(user.UserName, user.Id, "User");
                return Ok(token);
            }
            return Unauthorized();
        }
    }
}
