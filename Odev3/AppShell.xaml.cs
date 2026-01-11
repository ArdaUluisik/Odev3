using Odev3.Views;

namespace Odev3
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
            Routing.RegisterRoute(nameof(RegisterPage), typeof(RegisterPage));
            Routing.RegisterRoute(nameof(HaberDetayPage), typeof(HaberDetayPage));
            Routing.RegisterRoute(nameof(GorevDetayPage), typeof(GorevDetayPage));
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            await Shell.Current.GoToAsync("//LoginPage");
        }

        private async void OnLogoutClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//LoginPage");
        }
    }
}
