using Microsoft.Extensions.Logging;

namespace Odev3
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            // Servisleri Kaydet (Varsa)
            // builder.Services.AddSingleton<Services.FirebaseService>(); 

            // Sayfaları Kaydet (Best Practice)
            // Ana sayfalar sürekli açık kalacağı için Singleton olabilir
            builder.Services.AddSingleton<Views.LoginPage>();
            builder.Services.AddSingleton<Views.MainPage>();

            // Detay sayfaları her giriş çıkışta yenilensin diye Transient yapılır
            builder.Services.AddTransient<Views.GorevDetayPage>();
            builder.Services.AddTransient<Views.HaberDetayPage>();

            // Diğer sayfalar
            builder.Services.AddSingleton<Views.YapilacaklarPage>();
            builder.Services.AddSingleton<Views.HaberlerPage>();
            builder.Services.AddSingleton<Views.HavaDurumuPage>();
            builder.Services.AddSingleton<Views.KurlarPage>();
            builder.Services.AddSingleton<Views.AyarlarPage>();

            return builder.Build();
        }
    }
}
