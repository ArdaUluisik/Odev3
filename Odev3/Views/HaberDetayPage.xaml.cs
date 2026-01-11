using Odev3.Models;

namespace Odev3.Views
{
    [QueryProperty(nameof(SecilenHaber), "HaberObj")]
    public partial class HaberDetayPage : ContentPage
    {
        Haber haber;

        public Haber SecilenHaber
        {
            get => haber;
            set
            {
                haber = value;
                LoadData();
            }
        }

        public HaberDetayPage()
        {
            InitializeComponent();
        }

        void LoadData()
        {
            if (haber != null)
            {
                lblBaslik.Text = haber.Baslik;  
                imgHaber.Source = haber.Resim;  
                lblDetay.Text = haber.Ozet;     
                lblTarih.Text = haber.Tarih;    
            }
        }

        private async void Share_Clicked(object sender, EventArgs e)
        {
            if (haber == null) return;

            await Share.Default.RequestAsync(new ShareTextRequest
            {
                Uri = haber.Link,  
                Title = haber.Baslik 
            });
        }

        private async void OpenBrowser_Clicked(object sender, EventArgs e)
        {
            if (haber != null && !string.IsNullOrEmpty(haber.Link)) 
            {
                try
                {
                    await Browser.Default.OpenAsync(haber.Link, BrowserLaunchMode.SystemPreferred);
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Hata", "Link açýlamadý: " + ex.Message, "Tamam");
                }
            }
        }
    }
}