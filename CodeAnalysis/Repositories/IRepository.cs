using CodeAnalysis.Common.Models;
using Common.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repository
{
    public interface IRepository
    {
        public Task<IActionResult> Login(string name, string password);

        public Task<IActionResult> InsertUser(Users student);

        public Task<IActionResult> InsertReports(Reports report);

        public Task<IActionResult> getAllReports(string user_id, string repo_name);

        public Task<IActionResult> getReportById(string id);

    }
}
