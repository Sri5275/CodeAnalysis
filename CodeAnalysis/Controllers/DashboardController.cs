using CodeAnalysis.Common.Models;
using Common.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Octokit;
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


        [HttpGet]
        [Route("getAllReports")]
        public async Task<IActionResult> getAllReports(string user_id, string repo_name)
        {
            IActionResult actionResult = await _service.getAllReports(user_id, repo_name);
            return Ok(actionResult);           
        }

        [HttpGet]
        [Route("getReportById")]
        public async Task<IActionResult> getReportById(string id)
        {
            IActionResult actionResult = await _service.getReportById(id);
            return Ok(actionResult);
        }


    }

}

