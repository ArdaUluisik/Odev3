using Odev3.Models;
using System.Text.Json;
using System.Text;

namespace Odev3.Views
{
    public partial class YapilacaklarPage : ContentPage
    {
        List<Gorev> gorevler = new List<Gorev>();

        string baseUrl = "https://odev3-2566b-default-rtdb.firebaseio.com/";
        bool isYukleniyor = false;

        public YapilacaklarPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            VerileriYukle();
        }

        private async void VerileriYukle()
        {
            isYukleniyor = true;
            loading.IsVisible = true;
            loading.IsRunning = true;
            cvGorevler.ItemsSource = null;

            try
            {
                using var client = new HttpClient();
                if (string.IsNullOrEmpty(baseUrl) || !baseUrl.StartsWith("http")) return;

                var response = await client.GetStringAsync(baseUrl + "gorevler.json");

                if (response != "null" && !string.IsNullOrEmpty(response) && !response.Trim().StartsWith("<"))
                {
                    var data = JsonSerializer.Deserialize<Dictionary<string, Gorev>>(response);

                    gorevler = new List<Gorev>();
                    foreach (var item in data)
                    {
                        var gorev = item.Value;
                        gorev.Id = item.Key;
                        gorevler.Add(gorev);
                    }
                    gorevler = gorevler.OrderByDescending(x => x.Tarih).ToList();
                }
                else
                {
                    gorevler = new List<Gorev>();
                }
                cvGorevler.ItemsSource = gorevler;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Hata", "Veri çekilemedi: " + ex.Message, "Tamam");
            }
            finally
            {
                loading.IsRunning = false;
                loading.IsVisible = false;
                await Task.Delay(500);
                isYukleniyor = false;
            }
        }

        private async void BtnYeniGorev_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new GorevDetayPage());
        }

        private async void CvGorevler_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var secilen = e.CurrentSelection.FirstOrDefault() as Gorev;
            if (secilen == null) return;

            cvGorevler.SelectedItem = null;

            await Navigation.PushAsync(new GorevDetayPage(secilen));
        }

        private async void BtnSil_Clicked(object sender, EventArgs e)
        {
            var buton = sender as ImageButton;
            var silinecekGorev = buton.BindingContext as Gorev;

            if (silinecekGorev != null)
            {
                bool cevap = await DisplayAlert("Silme Onayı", $"'{silinecekGorev.Baslik}' görevini silmek istiyor musunuz?", "Evet, Sil", "Vazgeç");
                if (cevap)
                {
                    try
                    {
                        using var client = new HttpClient();
                        await client.DeleteAsync(baseUrl + $"gorevler/{silinecekGorev.Id}.json");
                        VerileriYukle(); 
                    }
                    catch
                    {
                        await DisplayAlert("Hata", "Silinemedi", "Tamam");
                    }
                }
            }
        }

        private async void CheckBox_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            if (isYukleniyor) return;

            var checkbox = sender as CheckBox;
            var guncellenecekGorev = checkbox.BindingContext as Gorev;

            if (guncellenecekGorev == null) return;

            guncellenecekGorev.YapildiMi = e.Value;

            try
            {
                using var client = new HttpClient();
                var json = JsonSerializer.Serialize(guncellenecekGorev);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                await client.PutAsync(baseUrl + $"gorevler/{guncellenecekGorev.Id}.json", content);
            }
            catch { }
        }
    }
}