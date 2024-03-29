using Microsoft.AspNetCore.Authentication.JwtBearer;
using ServerRA_AspnetCore.External.jsonRpc;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

Console.WriteLine(DateTime.Now);

//admin@firebase.com   admin1

//firebase real-time db https://computercompany-64270-default-rtdb.europe-west1.firebasedatabase.app

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
                        {
                            options.Authority = "https://securetoken.google.com/computercompany-64270";
                            options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                            {
                                ValidateIssuer = true,
                                ValidIssuer = "https://securetoken.google.com/computercompany-64270",
                                ValidateAudience = true,
                                ValidAudience = "computercompany-64270",
                                ValidateLifetime = true
                            };
                        }
);

builder.Services.AddJsonRpc();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.Map("/api/json-rpc", b => b.UseJsonRpc()
    );

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();


app.Run();
