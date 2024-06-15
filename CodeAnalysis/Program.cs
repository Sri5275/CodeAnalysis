using Microsoft.Azure.Cosmos;
using Repository.Repository;
using Service.Services;

var builder = WebApplication.CreateBuilder(args);
   
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.Services.AddScoped<IRepository, CosmosRepository>();

builder.Services.AddScoped<IService, CredentialsService>();


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


IConfiguration cosmosconfiguration = builder.Configuration.GetSection("CosmosDbConfiguration");
// Add services to the container.

builder.Services.AddSingleton<CosmosClient>((provider) =>
{
    var endpointuri = cosmosconfiguration["DatabaseUri"];
    var primarykey = cosmosconfiguration["Key"];
    var databaseId = cosmosconfiguration["DataBaseId"];
    var cosmosClient = new CosmosClient(endpointuri, primarykey);
    return cosmosClient;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
