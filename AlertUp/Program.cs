﻿
using AlertUp.Data;
using AlertUp.Model;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using AlertUp.Validator;
using AlertUp.Service.Implements;
using AlertUp.Service;

namespace AlertUp;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers()
            .AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            });

        //Conexão com o banco de dados
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));


        // Validação das Entidades
        builder.Services.AddTransient<IValidator<Tema>, TemaValidator>();
        builder.Services.AddTransient<IValidator<Postagem>, PostagemValidator>();
        //builder.Services.AddTransient<IValidator<User>, UserValidator>();

        // Registrar as Classes e Interfaces Service
        builder.Services.AddScoped<ITemaService, TemaService>();
        builder.Services.AddScoped<IPostagemService, PostagemService>();
        //builder.Services.AddScoped<IUserService, UserService>();


        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        //Configuração do CORS
        builder.Services.AddCors(options =>
        {
            options.AddPolicy(name: "MyPolicy",
            policy =>
            {
                policy.AllowAnyOrigin();
                policy.AllowAnyMethod();
                policy.AllowAnyHeader();
            });
        });

        var app = builder.Build();

        //Criar o banco de dados e as tabelas
        using (var scope = app.Services.CreateAsyncScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            dbContext.Database.EnsureCreated();
        }

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseCors("MyPolicy");

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}

