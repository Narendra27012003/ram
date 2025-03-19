#region References
using System;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using WebApiTemplate.Helpers;
using WebApiTemplate.Services;
using FluentValidation;
using FluentValidation.AspNetCore;
using WebApiTemplate.Data;
using WebApiTemplate.DTO;
using WebApiTemplate.Validators;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;


//using WebApiTemplate.Repository.Database;
//using WebApiTemplate.Repository.DatabaseOperation.Implementation;
//using WebApiTemplate.Repository.DatabaseOperation.Interface;
//using WebApiTemplate.Service;
//using WebApiTemplate.Service.Interface;
#endregion

var builder = WebApplication.CreateBuilder(args);

// Configure database context
//builder.Services.AddDbContext<WenApiTemplateDbContext>(
// options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));


// 🔹 Configure Swagger to support JWT Authentication
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "WebApiTemplate", Version = "v1" });

    // Add JWT Authentication to Swagger
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer {token}' (without quotes)"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

builder.Services.AddDbContext<WebApiTemplate.Data.ApplicationDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
    new MySqlServerVersion(new Version(8, 0, 21))));



builder.Services.AddValidatorsFromAssemblyContaining<UserRegisterValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<GenreValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<ReviewValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<BookFilterValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<BookValidator>();


// Register Authentication Services
builder.Services.AddScoped<AuthService>();
builder.Services.AddSingleton<JwtHelper>();
builder.Services.AddScoped<BookService>();
builder.Services.AddScoped<GenreService>();
builder.Services.AddScoped<ReviewService>();


// Add JWT Authentication

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Secret"])),
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
            ValidAudience = builder.Configuration["JwtSettings:Audience"]
        };
    });





//Add Automapper
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// Add services to the container.
builder.Services.AddControllers();

//inject Service layer
//builder.Services.AddScoped<IProductService, ProductService>();

//inject Data Access Layer - Repository
//builder.Services.AddScoped<IProductOperation, ProductOperation>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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










