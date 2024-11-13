using JWTAuthenticationForProduct.Controllers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using webapic_.Data;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<MyAppDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = context =>
            {
                var blacklistService = context.HttpContext.RequestServices.GetRequiredService<IJwtBlacklistService>();
                /*var token4 = context.SecurityToken;*/
                var authHeader = context.Request.Headers["Authorization"].ToString();
                var token = authHeader.StartsWith("Bearer ") ? authHeader.Substring("Bearer ".Length).Trim() : authHeader;
                /*if (context.SecurityToken is JwtSecurityToken jwtToken)
                {
                    // Retrieve the raw token string
                    var token2 = jwtToken.RawData;

                    if (blacklistService.IsTokenBlacklisted(token2))
                    {     
                        context.Fail("This token is blacklisted.");
                    }
                }*/
                //var token = context.SecurityToken as JwtSecurityToken;

                if (token != null && blacklistService.IsTokenBlacklisted(token))
                {
                    context.Fail("This token is blacklisted.");
                }

                return Task.CompletedTask;
            }
        };
    });
builder.Services.AddSingleton<IJwtBlacklistService, JwtBlacklistService>();

builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();

