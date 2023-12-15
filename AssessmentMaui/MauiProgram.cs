using AssessmentMaui.ViewModels;
using AssessmentMaui.Views;
using Microsoft.Extensions.Logging;

namespace AssessmentMaui
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
            builder.Services.AddSingleton<HomePage>();
            builder.Services.AddSingleton<LoginPage>();
            builder.Services.AddSingleton<SignupPage>();
            builder.Services.AddSingleton<Item>();
            builder.Services.AddSingleton<Views.Contact>();
            builder.Services.AddSingleton<LoginPageViewModel>();
            builder.Services.AddSingleton<SignupViewModel>();
#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}