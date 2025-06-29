using Microsoft.EntityFrameworkCore;
using MassTransit;
using PujaService.Aplicacion.Commands;
using PujaService.Dominio.Interfaces;
using PujaService.Infraestructura.Persistencia;
using PujaService.Infraestructura.Repositorios;
using PujaService.Infraestructura.Consumers;
using PujaService.Infraestructura.Mongo;
using PujaServicio.Infraestructura.Servicios;
using PujaService.Api.Hubs;
using PujaService.Api.Servicios;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// PostgreSQL (EF Core)
builder.Services.AddDbContext<PujaDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// MongoDB Context
<<<<<<< Updated upstream
builder.Services.AddSingleton<MongoDbContext>();
=======
builder.Services.AddSingleton<IMongoDbContext, MongoDbContext>();
builder.Services.AddScoped<IPujaMongoRepository, PujaMongoRepository>();
>>>>>>> Stashed changes

// Repositorio principal
builder.Services.AddScoped<IPujaRepository, PujaPostgresRepository>();



// Publisher que depende de IPublishEndpoint
builder.Services.AddScoped<IRabbitEventPublisher, RabbitEventPublisher>();

// MassTransit y configuración del consumidor
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<PujaRegistradaConsumer>();

    x.SetKebabCaseEndpointNameFormatter();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost");

        cfg.ReceiveEndpoint("puja-registrada-queue", e =>
        {
            e.ConfigureConsumer<PujaRegistradaConsumer>(context);
        });
    });
});

// MediatR para comandos y handlers
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(RegistrarPujaCommand).Assembly)
);

//SignalR
builder.Services.AddSignalR(); // Registra SignalR
builder.Services.AddScoped<INotificadorDePujas, SignalRNotificadorDePujas>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            .WithOrigins("http://localhost:3000") // Cambia si usas otro puerto
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});



var app = builder.Build();

// Swagger solo en entorno de desarrollo
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.MapHub<PujasHub>("/pujashub"); // Expone la ruta del WebSocket
app.UseHttpsRedirection();
app.UseCors("AllowFrontend");

app.UseAuthorization();
app.MapControllers();

app.Run();
app.MapHub<PujasHub>("/pujashub");
