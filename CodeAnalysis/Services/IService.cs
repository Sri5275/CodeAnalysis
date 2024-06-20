using Common.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services
{
    public interface IService
    {

        public Task<IActionResult> Login(LoginDTO loginDTO);
        public Task<IActionResult> Signup(SignupDTO student);
        public Task<IActionResult> InsertReports(Reports student);
        public Task<IActionResult> getAllReports(string user_id, string repo_name);
        public Task<IActionResult> getReportById(string id);

    }
}
