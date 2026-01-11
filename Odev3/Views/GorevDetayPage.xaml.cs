using Odev3.Models;
using System.Text.Json;
using System.Text;

namespace Odev3.Views
{
    public partial class GorevDetayPage : ContentPage
    {
       
        Gorev gelenGorev;

        string baseUrl = "https://odev3-2566b-default-rtdb.firebaseio.com/";

        public GorevDetayPage(Gorev gorev = null)
        {
            InitializeComponent();

            gelenGorev = gorev;

            if (gelenGorev != null)
            {
                Title = "Görevi Düzenle";
                entBaslik.Text = gelenGorev.Baslik;
                edtDetay.Text = gelenGorev.Detay;
                dpTarih.Date = gelenGorev.Tarih.Date;
                tpSaat.Time = gelenGorev.Tarih.TimeOfDay;
            }
            else
            {
                Title = "Yeni Görev Ekle";
                dpTarih.Date = DateTime.Now;
                tpSaat.Time = DateTime.Now.TimeOfDay;
            }
        }

        private async void BtnKaydet_Clicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(entBaslik.Text))
            {
                await DisplayAlert("Uyarı", "Başlık boş olamaz!", "Tamam");
                return;
            }

            loading.IsVisible = true;
            loading.IsRunning = true;

            try
            {
                DateTime tamTarih = dpTarih.Date + tpSaat.Time;

                using var client = new HttpClient();

                if (gelenGorev == null)
                {
                    var yeniGorev = new Gorev
                    {
                        Baslik = entBaslik.Text,
                        Detay = edtDetay.Text,
                        YapildiMi = false,
                        Tarih = tamTarih
                    };

                    var json = JsonSerializer.Serialize(yeniGorev);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    await client.PostAsync(baseUrl + "gorevler.json", content);
                }
                else
                {
                    gelenGorev.Baslik = entBaslik.Text;
                    gelenGorev.Detay = edtDetay.Text;
                    gelenGorev.Tarih = tamTarih;

                    var json = JsonSerializer.Serialize(gelenGorev);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
            
                    await client.PutAsync(baseUrl + $"gorevler/{gelenGorev.Id}.json", content);
                }

                await Navigation.PopAsync();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Hata", "Kaydedilemedi: " + ex.Message, "Tamam");
            }
            finally
            {
                loading.IsRunning = false;
                loading.IsVisible = false;
            }
        }

        private async void BtnIptal_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}