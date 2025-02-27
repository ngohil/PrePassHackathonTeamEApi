using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using PrePassHackathonTeamEApi;
using System.Text;


var builder = WebApplication.CreateBuilder(args);


builder.Services.Configure<AppSettings>(builder.Configuration.GetSection(nameof(AppSettings)));

//builder.Services.AddSingleton<MemoryCacheService>();
builder.Services.AddSingleton<FileDataService>();


// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
//builder.Services.AddOpenApi();

builder.Services.AddEndpointsApiExplorer(); // Add this line
builder.Services.AddSwaggerGen();

// Get the secret key from appsettings.json
var appSettings = builder.Configuration.GetSection("AppSettings").Get<AppSettings>();

var key = Encoding.UTF8.GetBytes(appSettings!.JwToken);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = false, // For testing, disable expiration
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };
    });

builder.Services.AddAuthorization();
// Add services to the container.
builder.Services.AddControllers();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    // Add development URLs
    app.Urls.Add("http://localhost:5000");
    app.Urls.Add("https://localhost:7019");
}
else
{
    // Production URLs
    app.Urls.Add("http://localhost:5000");
    app.Urls.Add("https://localhost:7019");
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
