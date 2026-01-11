using Odev3.Models;
using System.Text.Json;
using System.Globalization;

namespace Odev3.Views
{
    public class SehirHavaDurumu
    {
        public string Isim { get; set; }
        public double Lat { get; set; }
        public double Lon { get; set; }
    }

    public partial class HavaDurumuPage : ContentPage
    {
        // Ana Liste
        List<SehirHavaDurumu> sehirler = new List<SehirHavaDurumu>();

        string dosyaYolu = Path.Combine(FileSystem.AppDataDirectory, "sehirler.json");

        public HavaDurumuPage()
        {
            InitializeComponent();

            VerileriYukle();

            lblDosyaYolu.Text = "Kayıt Yeri: " + dosyaYolu;
        }

        private void VerileriYukle()
        {
            if (File.Exists(dosyaYolu))
            {
                string jsonText = File.ReadAllText(dosyaYolu);
                sehirler = JsonSerializer.Deserialize<List<SehirHavaDurumu>>(jsonText);
            }
            else
            {
                sehirler = new List<SehirHavaDurumu>
                {
                    new SehirHavaDurumu { Isim = "İstanbul", Lat = 41.0082, Lon = 28.9784 },
                    new SehirHavaDurumu { Isim = "Ankara", Lat = 39.9334, Lon = 32.8597 },
                    new SehirHavaDurumu { Isim = "İzmir", Lat = 38.4189, Lon = 27.1287 }
                };
                VerileriKaydet();
            }

            PickerGuncelle();
        }

        private void VerileriKaydet()
        {
            string jsonText = JsonSerializer.Serialize(sehirler);

            File.WriteAllText(dosyaYolu, jsonText);
        }

        private void PickerGuncelle()
        {
            pckSehirler.Items.Clear();
            foreach (var sehir in sehirler)
            {
                pckSehirler.Items.Add(sehir.Isim);
            }
        }

        private async void BtnEkle_Clicked(object sender, EventArgs e)
        {
            string aranacakSehir = entSehirAdi.Text?.Trim();
            if (string.IsNullOrEmpty(aranacakSehir))
            {
                await DisplayAlert("Uyarı", "Lütfen bir şehir adı yazın.", "Tamam");
                return;
            }

            lblDurumMesaji.Text = "Konum aranıyor...";
            lblDurumMesaji.IsVisible = true;
            entSehirAdi.IsEnabled = false; 

            try
            {
                using var client = new HttpClient();

                string url = $"https://geocoding-api.open-meteo.com/v1/search?name={aranacakSehir}&count=1&language=tr&format=json";

                var response = await client.GetStringAsync(url);
                var sonuc = JsonSerializer.Deserialize<GeoResponse>(response);

                if (sonuc != null && sonuc.results != null && sonuc.results.Count > 0)
                {
                    var bulunan = sonuc.results[0]; // İlk sonucu al

                    var yeniSehir = new SehirHavaDurumu
                    {
                        Isim = bulunan.name,
                        Lat = bulunan.latitude,
                        Lon = bulunan.longitude
                    };

                    if (sehirler.Any(s => s.Isim == yeniSehir.Isim))
                    {
                        await DisplayAlert("Bilgi", $"{yeniSehir.Isim} zaten listede var.", "Tamam");
                    }
                    else
                    {
                        sehirler.Add(yeniSehir);

                        VerileriKaydet();

                        PickerGuncelle();
                        await DisplayAlert("Başarılı", $"{yeniSehir.Isim} bulundu ve listeye eklendi!", "Tamam");

                        pckSehirler.SelectedIndex = sehirler.Count - 1;
                    }

                    entSehirAdi.Text = ""; 
                }
                else
                {
                    await DisplayAlert("Bulunamadı", "Böyle bir şehir bulunamadı. Yazımı kontrol edin.", "Tamam");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Hata", "Bağlantı hatası: " + ex.Message, "Tamam");
            }
            finally
            {
                lblDurumMesaji.IsVisible = false;
                entSehirAdi.IsEnabled = true;
            }
        }

        private async void PckSehirler_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = pckSehirler.SelectedIndex;
            if (index == -1) return;

            var secilen = sehirler[index];
            await HavaDurumuGetir(secilen.Lat, secilen.Lon);
        }

        private async Task HavaDurumuGetir(double lat, double lon)
        {
            loading.IsVisible = true;
            loading.IsRunning = true;
            cvHavaDurumu.ItemsSource = null;

            try
            {
                string url = $"https://api.open-meteo.com/v1/forecast?latitude={lat.ToString().Replace(",", ".")}&longitude={lon.ToString().Replace(",", ".")}&daily=weathercode,temperature_2m_max,temperature_2m_min&timezone=auto";

                using var client = new HttpClient();
                var response = await client.GetStringAsync(url);
                var veri = JsonSerializer.Deserialize<WeatherResponse>(response);

                if (veri != null && veri.daily != null)
                {
                    List<GunlukTahmin> tahminListesi = new List<GunlukTahmin>();
                    for (int i = 0; i < veri.daily.time.Length; i++)
                    {
                        DateTime tarih = DateTime.Parse(veri.daily.time[i]);
                        string gunIsmi = tarih.ToString("dddd", new CultureInfo("tr-TR"));
                        if (tarih.Date == DateTime.Today) gunIsmi = "Bugün";

                        double max = veri.daily.temperature_2m_max[i];
                        double min = veri.daily.temperature_2m_min[i];
                        string sicaklik = $"{Math.Round(max)}° / {Math.Round(min)}°";

                        int kod = veri.daily.weathercode[i];
                        var (durumYazisi, resimLink) = HavaKodunuCoz(kod);

                        tahminListesi.Add(new GunlukTahmin
                        {
                            GunIsmi = gunIsmi,
                            Tarih = tarih.ToString("dd.MM.yyyy"),
                            Sicaklik = sicaklik,
                            Durum = durumYazisi,
                            Resim = resimLink
                        });
                    }
                    cvHavaDurumu.ItemsSource = tahminListesi;
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Hata", "Hava durumu alınamadı: " + ex.Message, "Tamam");
            }
            finally
            {
                loading.IsRunning = false;
                loading.IsVisible = false;
            }
        }

        private (string, string) HavaKodunuCoz(int code)
        {
            if (code == 0) return ("Güneşli", "https://cdn-icons-png.flaticon.com/512/869/869869.png");
            if (code >= 1 && code <= 3) return ("Parçalı Bulutlu", "https://cdn-icons-png.flaticon.com/512/1163/1163624.png");
            if (code >= 45 && code <= 48) return ("Sisli", "https://cdn-icons-png.flaticon.com/512/4005/4005901.png");
            if (code >= 51 && code <= 67) return ("Yağmurlu", "https://cdn-icons-png.flaticon.com/512/1163/1163657.png");
            if (code >= 71 && code <= 77) return ("Karlı", "https://cdn-icons-png.flaticon.com/512/642/642102.png");
            if (code >= 95) return ("Fırtına", "https://cdn-icons-png.flaticon.com/512/1146/1146860.png");
            return ("Bulutlu", "https://cdn-icons-png.flaticon.com/512/1163/1163624.png");
        }
    }
}