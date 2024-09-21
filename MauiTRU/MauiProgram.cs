using LibraryTRU.IServices;
using MauiTRU.Database;
using MauiTRU.Services;
using Microsoft.Extensions.Logging;
using System.Runtime.CompilerServices;
using ZXing.Net.Maui.Controls;

namespace MauiTRU
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new(Preferences.Get(Constants.PreferenceKeyForAPI, Constants.ProductionDefault));
            // client.BaseAddress = new(Preferences.Get(Constants.PreferenceKeyForAPI, Constants.LocalHostDefault));
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseBarcodeReader()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            builder.Services.AddMauiBlazorWebView();
            builder.Services.AddScoped<LocalTRUDatabase>();
            builder.Services.AddSingleton<IDbPath, MauiDbPath>();
            builder.Services.AddScoped<ITicketService, MauiTicketService>();
            builder.Services.AddScoped<IConcertService, MauiConcertService>();
            builder.Services.AddSingleton(client);
            builder.Services.AddSingleton<BackgroundTimerService>();
            builder.Services.AddSingleton<ConnectivityForPhone>();

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
    		builder.Logging.AddDebug();
#endif
            return builder.Build();
        }
    }
}
