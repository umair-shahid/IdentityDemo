using IdentityDemo.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using IdentityDemo.Service.Interface;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace IdentityDemo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]

    public class AppRegistrationController : Controller
    {
        private readonly IAppRegistrationService _appRegService;

        public AppRegistrationController(IAppRegistrationService appRegService)
        {
            _appRegService = appRegService;
        }


        [HttpGet("GetRegisteredApps")]
        public async Task<IActionResult> GetRegisteredApps()
        {
            try
            {
                var res = await _appRegService.GetRegisteredApps();
                if (!res.Any())
                {
                    return Ok( new Response { Success = true, Message = "No any app exist" });
                }
                return Ok(new Response { Success=true, Message = "List of registered apps!", Data = res});

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Success = false, Message = "Something went wrong", Data = ex.Message });
            }
        }

        [HttpPost("RegisterAppAndGetKey")]
        public async Task<IActionResult> RegisterAppAndGetKey(ApplicationRegisteration model)
        {
            try
            {
                ApplicationRegisteration res = await _appRegService.RegisterAppAndGetKey(model);
                if (res==null)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new Response { Success = false, Message = "App already exist with this name" });
                }
                return Ok(new Response { Status = "Success", Success=true, Message = "App created successfully!", Data = res });

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Success = false, Message = "Something went wrong", Data = ex.Message });
            }
        }

        [HttpPost("GenerateToken")]
        public async Task<IActionResult> GenerateToken(TokenGeneration model)
        {
            try
            {
                var isExist = await _appRegService.IsAppExist(model.AppName);
                if (isExist)
                {
                    var res = await _appRegService.GenerateToken(model);
                    if (res != null)
                    {
                        return Ok(new Response
                        {
                            Message = "Token generated successfully",
                            Data = res,
                            Success = true
                        });
                    }
                    return StatusCode(StatusCodes.Status500InternalServerError,
                        new Response
                        {
                            Message = "Provided detail is not valid for this app or secret key has been expired",
                            Data = res,
                            Success = false
                        });
                }

                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Success = false, Message = "App not exist" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Success = false, Message = "Something went wrong", Data = ex.Message });
            }
            
        }
    }
}
