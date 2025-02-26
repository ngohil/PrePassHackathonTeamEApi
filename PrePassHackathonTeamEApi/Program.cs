var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
//builder.Services.AddOpenApi();

builder.Services.AddEndpointsApiExplorer(); // Add this line
builder.Services.AddSwaggerGen();  

builder.Services.AddMemoryCache();

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

app.MapControllers();

app.Run();
