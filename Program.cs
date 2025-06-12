using MapController.Persistence;
using robot_controller_api.Persistence;
using FullImplementaionAPI.Persistence;
using Microsoft.OpenApi.Models;
using System.Reflection;
using FullImplementaionAPI.Authentication;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Register data access interface and implementation
builder.Services.AddScoped<IMapCommandDataAccess, MapRepository>(); 
builder.Services.AddScoped<IRobotCommandDataAccess, RobotCommandRepository>();
builder.Services.AddScoped<IUserDataAccess, UserRepository>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Robot Controller API",
        Description = "New backend service that provides resources for the Moon robot simulator.",
        Contact = new OpenApiContact
        {
            Name = "Cooper Goullet", 
            Email = "s222326285@deakin.edu.au" 
        }
    });
});

builder.Services.AddAuthentication("BasicAuthentication")
    .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuthentication", default);

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy =>
    policy.RequireClaim(ClaimTypes.Role, "Admin"));
    options.AddPolicy("UserOnly", policy =>
    policy.RequireClaim(ClaimTypes.Role, "Admin", "User"));
});


var app = builder.Build();

app.UseStaticFiles();
app.UseSwagger();
app.UseSwaggerUI(setup =>
{
    setup.InjectStylesheet("/styles/theme-flattop.css"); 
});

app.UseAuthentication();
app.UseAuthorization();



app.UseHttpsRedirection();

app.MapControllers();

app.Run();

