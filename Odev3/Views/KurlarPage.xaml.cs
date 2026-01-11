using Newtonsoft.Json.Linq;
using Odev3.Models;
using System.Collections.Generic;

namespace Odev3.Views
{
    public partial class KurlarPage : ContentPage
    {
        public KurlarPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            DovizleriGetir();
        }

        private async void DovizleriGetir()
        {
            loading.IsVisible = true;
            loading.IsRunning = true;

            cvKurlar.ItemsSource = null;

            try
            {
                await Task.Delay(2000);
               

                string url = "https://finans.truncgil.com/today.json";
                using var client = new HttpClient();
                string jsonVerisi = await client.GetStringAsync(url);

                var data = JObject.Parse(jsonVerisi);

                List<KurItem> kurListesi = new List<KurItem>();

                foreach (var item in data)
                {
                    if (item.Key == "Update_Date") continue;

                    var detaylar = item.Value;
                    string degisimOrani = detaylar["Deðiþim"]?.ToString();

                    Color durumRengi = Colors.Green;
                    if (degisimOrani.Contains("-")) durumRengi = Colors.Red;
                    else if (degisimOrani == "%0.00") durumRengi = Colors.Gray;

                    kurListesi.Add(new KurItem
                    {
                        Name = item.Key,
                        Alis = detaylar["Alýþ"]?.ToString(),
                        Satis = detaylar["Satýþ"]?.ToString(),
                        Degisim = degisimOrani,
                        Renk = durumRengi
                    });
                }

                cvKurlar.ItemsSource = kurListesi;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Hata", "Veri çekilemedi: " + ex.Message, "Tamam");
            }
            finally
            {
                loading.IsRunning = false;
                loading.IsVisible = false;
            }
        }
    }
}