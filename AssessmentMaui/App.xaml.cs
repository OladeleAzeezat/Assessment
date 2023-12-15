using AssessmentMaui.Model;
using System.Security.Cryptography.X509Certificates;

namespace AssessmentMaui
{
    public partial class App : Application
    {
        public static Employee employee;
        public static string Token;

        public App()
        {
            InitializeComponent();

            MainPage = new AppShell();
        }
    }
}