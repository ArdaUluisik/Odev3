using Odev3.Services;

namespace Odev3.Views
{
    public partial class RegisterPage : ContentPage
    {
        public RegisterPage()
        {
            InitializeComponent();

            Shell.Current.FlyoutBehavior = FlyoutBehavior.Disabled;
        }

        private async void BtnKaydol_Clicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(entName.Text) ||
                string.IsNullOrWhiteSpace(entEmail.Text) ||
                string.IsNullOrWhiteSpace(entPass.Text))
            {
                await DisplayAlert("Hata", "Tüm alanları doldurun", "OK");
                return;
            }

            var res = await FirebaseService.Register(
                entName.Text,
                entEmail.Text,
                entPass.Text);

            if (res)
            {
                await DisplayAlert("Başarılı",
                    "Kayıt olundu, giriş yapabilirsiniz.",
                    "OK");

                await Shell.Current.GoToAsync("//LoginPage");
            }
            else
            {
                await DisplayAlert("Hata",
                    "Kayıt başarısız.",
                    "OK");
            }
        }

        private async void BtnLoginDon_Clicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//LoginPage");
        }
    }
}
