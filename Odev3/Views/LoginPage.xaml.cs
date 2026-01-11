using Odev3.Services;

namespace Odev3.Views
{
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        private async void BtnLogin_Clicked(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(entEmail.Text) || string.IsNullOrEmpty(entPass.Text))
            {
                await DisplayAlert("Hata", "Alanları doldurun", "OK");
                return;
            }

            var uid = await FirebaseService.Login(entEmail.Text, entPass.Text);

            if (uid != null)
            {
                Shell.Current.FlyoutBehavior = FlyoutBehavior.Flyout;

                await Shell.Current.GoToAsync("//MainPage");
            }
            else
            {
                await DisplayAlert("Hata", "Giriş başarısız", "OK");
            }
        }


        private async void BtnRegister_Clicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(RegisterPage));
        }
    }
}