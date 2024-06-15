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
                        return new OkObjectResult(new { message = "successful login ", user_id = existingDocument["user_id"].ToString() });
                        // return new { success = true, user_id = existingDocument["id"].ToString(), user_name = existingDocument["name"].ToString() };
                    }
                    else
                    {
                        return new OkObjectResult(new { message = "Password incorrect .Please enter correct password" });
                    }
                }
                else
                {
                    return new OkObjectResult(new { message = "User not found  " });
                }
            }
            catch (Exception ex)
            {
                return new OkObjectResult(new { message = ex.Message });
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
                return new OkObjectResult(new { success = true, user_id = existingDocument.user_id, username = existingDocument.username });
            }
            else
            {
                await _container.CreateItemAsync(user);
                return new OkObjectResult(new { success = true, message = "Account Created." });
            }
        }

        public async Task<IActionResult> InsertReports(Reports report)
        {
            await _container.CreateItemAsync(report);
            return new OkObjectResult(new { success = true, message = "Report inserted successfully" });
        }


    }
}
