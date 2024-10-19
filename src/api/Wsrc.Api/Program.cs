using Wsrc.Api.Business.Filters.Validations;
using Wsrc.Api.Business.Services;
using Wsrc.Api.Endpoints;
using Wsrc.Domain.Repositories;

namespace Wsrc.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddDbContext<WsrcDbContext>();

        builder.Services.AddScoped<ChatroomRepository>();
        builder.Services.AddScoped<ChatroomService>();
        builder.Services.AddSingleton<ValidationUtilities>();

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        EndpointConfigurator.Configure(app);
        
        app.Run();
    }
}