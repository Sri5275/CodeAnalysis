using CodeAnalysis.Common.Models;
using Common.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repository
{
    public class CosmosRepository : IRepository
    {
        private readonly CosmosClient _cosmosClient;
        private readonly Database _database;
        private readonly Container _container;
        public CosmosRepository(CosmosClient cosmosClient)
        {
            _cosmosClient = cosmosClient;
            _database = _cosmosClient.GetDatabase("coderesultdb");
            _container = _database.GetContainer("codeanalysis");
        }

        public async Task<IActionResult> Login(string name, string hashedPassword)
        {
            try
            {
                // Check if a document with the provided name exists
                var query = new QueryDefinition("SELECT * FROM c WHERE c.username = @name AND c.type='Users'")
                                .WithParameter("@name", name);
                var iterator = _container.GetItemQueryIterator<dynamic>(query);
                var existingDocument = (await iterator.ReadNextAsync()).FirstOrDefault();

                if (existingDocument != null)
                {
                    // Username exists, check if the password matches
                    string storedPassword = existingDocument["password"].ToString();
                    if (hashedPassword == storedPassword)
                    {
                        return new OkObjectResult(new { message = "successful login ", user_id = existingDocument["user_id"].ToString(), success =true });
                        // return new { success = true, user_id = existingDocument["id"].ToString(), user_name = existingDocument["name"].ToString() };
                    }
                    else
                    {
                        return new OkObjectResult(new { message = "Password incorrect .Please enter correct password" , success=false});
                    }
                }
                else
                {
                    return new OkObjectResult(new { message = "User not found  ", success = false });
                }
            }
            catch (Exception ex)
            {
                return new OkObjectResult(new { message = ex.Message , success = false});
            }
        }

        public async Task<IActionResult> InsertUser(Users user)
        {
            // Check if a document with the same username already exists
            var query = new QueryDefinition("SELECT * FROM c WHERE c.username = @name AND c.type='Users'")
                            .WithParameter("@name", user.username);
            var iterator = _container.GetItemQueryIterator<Users>(query);
            var existingDocument = (await iterator.ReadNextAsync()).FirstOrDefault();

            if (existingDocument != null)
            {
                // return new { success = false, message = "UserName already exists." };
                return new OkObjectResult(new { message="username already taken",success = false, user_id = existingDocument.user_id, username = existingDocument.username });
            }
            else
            {
                await _container.CreateItemAsync(user);
                return new OkObjectResult(new { success = true,user_id = user.user_id ,message = "Account Created." });
            }
        }

        public async Task<IActionResult> InsertReports(Reports report)
        {
            await _container.CreateItemAsync(report);
            return new OkObjectResult(new { success = true, message = "Report inserted successfully" });
        }

        public async Task<IActionResult> getAllReports(string user_id, string repo_name)
        {

            try
            {
                List<ReportSummary> reportSummaries = new List<ReportSummary>();


                QueryDefinition query = new QueryDefinition("SELECT c.id, c.user_id, c.repo_name, c.timestamp, c.avgScore, c.criticalErrors " +
                                                   "FROM c WHERE c.repo_name = @repo_name AND c.user_id = @user_id AND c.type = 'reports'")
                                   .WithParameter("@repo_name", repo_name)
                                   .WithParameter("@user_id", user_id);

                FeedIterator<ReportSummary> iterator = _container.GetItemQueryIterator<ReportSummary>(query);

                while (iterator.HasMoreResults)
                {
                    FeedResponse<ReportSummary> res = await iterator.ReadNextAsync();
                    reportSummaries.AddRange(res.Resource);
                }
                if (reportSummaries.Count == 0)
                {
                    return new OkObjectResult(new { message = "No reports found. Please enter the valid credentials", success = false });
                }

                return new OkObjectResult(new { payload=reportSummaries , success=true});
            }
            catch (Exception ex)
            {
                return new ObjectResult(new { message = ex.Message , success = false}) { StatusCode = 500 };
            }   
        }

        public async Task<IActionResult> getReportById(string id)
        {
            try
            {
                ReportContainer reportContainer = null;

                QueryDefinition query = new QueryDefinition("SELECT * FROM c WHERE c.id = @id")
                                           .WithParameter("@id", id);

                FeedIterator<ReportContainer> iterator = _container.GetItemQueryIterator<ReportContainer>(query);


                if (iterator.HasMoreResults)
                {
                    FeedResponse<ReportContainer> response = await iterator.ReadNextAsync();
                    reportContainer = response.FirstOrDefault(); // There should be only one item with the given ID
                }

                if (reportContainer == null)
                {
                    return new NotFoundObjectResult(new { message = "Report not found" , success = false});
                }

                return new OkObjectResult(new { payload = reportContainer, success = true });
            }
            catch (Exception ex)
            {
                return new ObjectResult(new { message = ex.Message, success = false }) { StatusCode = 500 };
            }
        }
    }
}
