using Common.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Repository.Repository;
using Service.Services;

namespace CodeAnalysis.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {

        private readonly IService _service;

        public DashboardController(IService service)
        {
            _service = service;

        }



        [HttpPost]
        [Route("InsertUser")]
        public async Task<dynamic> Signup(SignupDTO signupDTO)
        {

            try
            {
                return await _service.Signup(signupDTO);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return new { success = false, message = ex.Message };
            }
        }


        [HttpPost]
        [Route("Login")]
        public async Task<dynamic> Login(LoginDTO loginDTO)
        {
            try
            {
                return await _service.Login(loginDTO);
            }
            catch (Exception e)
            {
                return new { success = false, message = e.Message };
            }
        }

        public static implicit operator DashboardController(CredentialsService v)
        {
            throw new NotImplementedException();
        }






    }

}
