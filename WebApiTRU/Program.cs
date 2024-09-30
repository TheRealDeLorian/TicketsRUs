using Microsoft.EntityFrameworkCore;
using WebApiTRU.Components;
using WebApiTRU.Email;
using WebApiTRU.Services;

public class Program
{
    private static void Main(string[] args)
    {

        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddRazorComponents()
            .AddInteractiveServerComponents();

        builder.Services.AddControllers();
        builder.Services.AddDbContextFactory<PostgresContext>(options =>
        {
            options.UseNpgsql(builder.Configuration.GetConnectionString("db"),
                              b => b.MigrationsAssembly("WebApiTRU"));
        }, ServiceLifetime.Scoped);



        builder.Services.AddScoped<IConcertService, ConcertService>();
        builder.Services.AddScoped<ITicketService, TicketService>();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddTransient<IEmailService, EmailService>();
        builder.Services.AddServerSideBlazor().AddCircuitOptions(options => { options.DetailedErrors = true; });

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseExceptionHandler("/Error", createScopeForErrors: true);
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();

        app.UseStaticFiles();
        app.UseAntiforgery();

        app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode();

        app.MapControllers();

        app.Run();
    }
}
