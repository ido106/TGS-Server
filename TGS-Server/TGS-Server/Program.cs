using DatabaseLibrary;
using Domain.DatabaseSettings;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Services.UserServices;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.Configure<TgsSolverDatabaseSettings>(
    builder.Configuration.GetSection(nameof(TgsSolverDatabaseSettings)));

builder.Services.AddSingleton<ITgsSolverDatabaseSettings>(sp =>
    sp.GetRequiredService<IOptions<TgsSolverDatabaseSettings>>().Value);

builder.Services.AddSingleton<IMongoClient>(s =>
    new MongoClient(builder.Configuration.GetValue<string>("TgsSolverDatabaseSettings:ConnectionString")));


builder.Services.AddSingleton<Database>();
builder.Services.AddControllers();

// add services
builder.Services.AddScoped<IAnswerService, AnswerService>();
builder.Services.AddScoped<ILoginService, LoginService>();

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});


var app = builder.Build();
// Use Google Authentication
app.UseAuthentication();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();