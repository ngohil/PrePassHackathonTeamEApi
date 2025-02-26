using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using PrePassHackathonTeamEApi;
using System.Text;

string key = "XOxQFDzAjOJeeHGSRYMmZZntyVg/IuKL4H0aw=ghNhaF4zDOT81aS7Maf28wmc0mMp=Gqx1o22m4xXE7lRBfxKMkR9FDj0wbZPG6BlxNdGF-Rca3pgGOTn5UMfFM?aT6VsjP6/yJpFgbW8QmTpMMuxaZcHrPxE?Cs0qZ6zRIkU3P=?4Xnrj7RKdY029F8Lw!nJ00QzB2ADqLW7bqdr6DNpp7mu1G=dxmpvw1q5Hs6k9EQ-jt-vHPq5?fJkhv0oyE";
string filePath = "data.json";
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<MemoryCacheService>();


// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
//builder.Services.AddOpenApi();

builder.Services.AddEndpointsApiExplorer(); // Add this line
builder.Services.AddSwaggerGen();

//builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//    .AddJwtBearer(options =>
//    {
//        options.TokenValidationParameters = new TokenValidationParameters
//        {
//            ValidateIssuer = false,
//            ValidateAudience = false,
//            ValidateLifetime = false, // For testing, disable expiration
//            ValidateIssuerSigningKey = true,
//            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
//        };
//    });
// Add services to the container.
builder.Services.AddControllers();

/* builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = "https://your-auth-provider.com";
        options.Audience = "your-api-audience";
    });
 */

var app = builder.Build();

// Configure the HTTP request pipeline.
/* if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
} */

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
//app.UseAuthentication();

app.MapControllers();

app.Run();
