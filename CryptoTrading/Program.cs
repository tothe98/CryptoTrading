using CryptoTrading.DataContext;
using CryptoTrading.Extensions;
using CryptoTrading.Services.Security;
using CryptoTrading.Services.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddOpenApi();
//DB Connection
builder.Services.AddDbContext<SQL>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SQL")));

//Services
builder.Services.AddLocalServices();

//JWT Token
byte[] JWTKey = Encoding.UTF8.GetBytes(builder.Configuration["JWT:JWTKey"]);
string? Audience = builder.Configuration["JWT:Audience"];
string? Issuer = builder.Configuration["JWT:Issuer"];
builder.Services.AddScoped<TokenHandlerService>(sp => new TokenHandlerService(JWTKey, Issuer, Audience));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = Issuer,
            ValidAudience = Audience,
            IssuerSigningKey = new SymmetricSecurityKey(JWTKey)
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("User", policy => policy.RequireClaim(ClaimTypes.Role, "User"));
    options.AddPolicy("Admin", policy => policy.RequireClaim(ClaimTypes.Role, "Admin"));
});

builder.Services.AddCors();

//swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "CryptoTrading", Version = "v1" });


    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please insert JWT token",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
                {
                    new OpenApiSecurityScheme {
                        Reference = new OpenApiReference {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new string[] { }
                }});
});


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    var scope = app.Services.CreateScope();
    var seeder = scope.ServiceProvider.GetRequiredService<IInitialService>();
    await seeder.Seed();
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "CryptoTrading API");
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.UseCors(options =>
{
    options.AllowAnyMethod();
    options.AllowAnyOrigin();
    options.AllowAnyHeader();
});

app.MapControllers();

app.Run();
