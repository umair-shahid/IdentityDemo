using IdentityDemo.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using System.Linq;
using IdentityDemo.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace IdentityDemo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class AppRegistrationController : Controller
    {
        private readonly AppRegistrationService _appRegService;

        public AppRegistrationController(DbContext context, IConfiguration config)
        {
            _appRegService = new AppRegistrationService(context, config);
        }


        [HttpGet("GetRegisteredApps")]
        public async Task<IActionResult> GetRegisteredApps()
        {
            try
            {
                var res = await _appRegService.GetRegisteredApps();
                if (!res.Any())
                {
                    return Ok( new Response { Success = true, Status = "Success", Message = "No any app exist" });
                }
                return Ok(new Response { Success=true,Status = "Success", Message = "List of registered apps!", Data = res});

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Something went wrong", Data = ex.Message });
            }
        }

        [HttpPost("RegisterApp")]
        public async Task<IActionResult> RegisterApp(ApplicationRegisteration model)
        {
            try
            {
                ApplicationRegisteration res = await _appRegService.Register(model);
                if (res==null)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "App already exist with this name" });
                }
                return Ok(new Response { Status = "Success", Message = "App created successfully!", Data = new ApplicationRegisteration() { AppName = res.AppName, ClientId = res.ClientId, SecretKey = res.SecretKey } });

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Something went wrong", Data = ex.Message });
            }
        }

        [HttpPost("GenerateToken")]
        public async Task<IActionResult> GenerateToken(TokenGeneration model)
        {
            var isExist = await _appRegService.IsAppExist(model.AppName);
            if (isExist)
            {
                var res = await _appRegService.GenerateToken(model);
                if (res != null)
                {
                    return Ok(new Response
                    {
                        Status = "Success",
                        Message = "Token generated successfully",
                        Data = res,
                        Success = true
                    });
                }
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new Response { 
                    Status = "Fail",
                    Message = "Provided detail is not valid for this app",
                    Data = res,
                    Success = false
                });
            }

            return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Success", Message = "App not exist" });
        }
    }
}
