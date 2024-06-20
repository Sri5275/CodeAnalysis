using Common.Models;
using Microsoft.AspNetCore.Mvc;
using Repository.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using Octokit;
using Microsoft.Azure.Cosmos;
using CodeAnalysis.Common.Models;


namespace Service.Services
{
    public class CredentialsService : IService
    {

        private readonly IRepository _repos;

        public CredentialsService(IRepository repo)
        {

            _repos = repo;
        }



        //insert  and HASH PASSWORD;
        public async Task<IActionResult> Signup(SignupDTO loginDTO)
        {
            string id = Guid.NewGuid().ToString();
            string type = "Users";
            Users newUser = new Users
            {
                id = id,
                type = type,
                username = loginDTO.username,
                password = loginDTO.password,
                email = loginDTO.email,
                user_id = loginDTO.user_id,
            };



            string HashPassword(string password)
            {
                using (SHA256 sha256Hash = SHA256.Create())
                {
                    byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));

                    StringBuilder builder = new StringBuilder();
                    for (int i = 0; i < bytes.Length; i++)
                    {
                        builder.Append(bytes[i].ToString("x2"));
                    }
                    return builder.ToString();
                }
            }

            string hashedPassword = HashPassword(loginDTO.password);
            newUser.password = hashedPassword;
            try
            {
                return await _repos.InsertUser(newUser);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return new OkObjectResult(new { success = false, message = e.Message });
            }
        }


        //LOGIN CHecking details and hashed passwords;

        public async Task<IActionResult> Login(LoginDTO loginDTO)
        {
            string HashPassword(string password)
            {
                using (SHA256 sha256Hash = SHA256.Create())
                {
                    byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));

                    StringBuilder builder = new StringBuilder();
                    for (int i = 0; i < bytes.Length; i++)
                    {
                        builder.Append(bytes[i].ToString("x2"));
                    }
                    return builder.ToString();
                }
            }
            string hashedPassword = HashPassword(loginDTO.password);

            try
            {
                var res = await _repos.Login(loginDTO.username, hashedPassword);
                Console.WriteLine(res);
                return res;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return new OkObjectResult(new { success = false, message = ex.Message });
            }
        }



        public async Task<IActionResult> InsertReports(Reports report)
        {


            await _repos.InsertReports(report);

            return new OkObjectResult(new { success = true, message = "Report sent to repository successfully" });


        }


        public async Task<IActionResult> getAllReports(string user_id, string repo_name)
        {
            return await _repos.getAllReports(user_id, repo_name);
        }

        public async Task<IActionResult> getReportById(string id)
        {
            return await _repos.getReportById(id);
        }

    }
}
