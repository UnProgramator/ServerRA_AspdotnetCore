using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);

//admin@firebase.com   admin1

//firebase real-time db https://computercompany-64270-default-rtdb.europe-west1.firebasedatabase.app

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
                                                                                        {
                                                                                            options.Authority = "https.https://securetoken.google.com/1:449726311356:web:6b06b1126f37de8975d9b9";
                                                                                            options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters {
                                                                                                ValidateIssuer = true,
                                                                                                ValidIssuer = "https.https://securetoken.google.com/1:449726311356:web:6b06b1126f37de8975d9b9",
                                                                                                ValidateAudience = true,
                                                                                                ValidAudience = "1:449726311356:web:6b06b1126f37de8975d9b9",
                                                                                                ValidateLifetime = true
                                                                                            };
                                                                                        }
);

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
