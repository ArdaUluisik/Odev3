using Odev3.Models;
using System.Xml.Linq;
using System.Text.RegularExpressions;

namespace Odev3.Views
{
    public class Kategori
    {
        public string Baslik { get; set; }
        public string Link { get; set; }
    }

    public partial class HaberlerPage : ContentPage
    {
        List<Kategori> kategoriListesi = new List<Kategori>
        {
            new Kategori { Baslik = "ANA SAYFA", Link = "https://www.cumhuriyet.com.tr/rss" },
            new Kategori { Baslik = "GÜNDEM", Link = "https://www.cumhuriyet.com.tr/rss/turkiye" },
            new Kategori { Baslik = "SPOR", Link = "https://www.cumhuriyet.com.tr/rss/spor" },
            new Kategori { Baslik = "EKONOMİ", Link = "https://www.cumhuriyet.com.tr/rss/ekonomi" },
            new Kategori { Baslik = "DÜNYA", Link = "https://www.cumhuriyet.com.tr/rss/dunya" },
            new Kategori { Baslik = "BİLİM/TEK", Link = "https://www.cumhuriyet.com.tr/rss/bilim-teknoloji" },
            new Kategori { Baslik = "SAĞLIK", Link = "https://www.cumhuriyet.com.tr/rss/saglik" },
            new Kategori { Baslik = "YAŞAM", Link = "https://www.cumhuriyet.com.tr/rss/yasam" }
        };

        public HaberlerPage()
        {
            InitializeComponent();
            cvKategoriler.ItemsSource = kategoriListesi;

            HaberleriGetir(kategoriListesi[0].Link);
        }

        private void CvKategoriler_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.Count == 0) return;

            var secilenKategori = e.CurrentSelection.FirstOrDefault() as Kategori;
            if (secilenKategori != null)
            {
                HaberleriGetir(secilenKategori.Link);
                ((CollectionView)sender).SelectedItem = null;
            }
        }

        private async void HaberleriGetir(string url)
        {
            loading.IsVisible = true;
            loading.IsRunning = true;
            cvHaberler.ItemsSource = null;

            try
            {
                await Task.Delay(200);

                using var client = new HttpClient();
                client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64)");

                string xmlData = await client.GetStringAsync(url);
                XDocument doc = XDocument.Parse(xmlData);

              
                XNamespace media = "http://search.yahoo.com/mrss/";

                List<Haber> haberler = new List<Haber>();

                foreach (var item in doc.Descendants("item"))
                {
                    // Varsayılan Logo
                    string resimUrl = "https://www.cumhuriyet.com.tr/Archive/2021/9/23/1024x662/cumhuriyet-logo.jpg";

                    string baslik = item.Element("title")?.Value;
                    string link = item.Element("link")?.Value;
                    string aciklama = item.Element("description")?.Value;
                    string tarih = item.Element("pubDate")?.Value;

                    var mediaContent = item.Element(media + "content");
                    if (mediaContent != null)
                    {
                        var urlAttr = mediaContent.Attribute("url");
                        if (urlAttr != null)
                        {
                            resimUrl = urlAttr.Value;
                        }
                    }

                    if (resimUrl.Contains("cumhuriyet-logo"))
                    {
                        var enclosure = item.Element("enclosure");
                        if (enclosure != null)
                        {
                            var urlAttr = enclosure.Attribute("url");
                            if (urlAttr != null) resimUrl = urlAttr.Value;
                        }
                    }

                   
                    if (resimUrl.Contains("cumhuriyet-logo") && !string.IsNullOrEmpty(aciklama))
                    {
                        string decodeEdilmis = System.Net.WebUtility.HtmlDecode(aciklama);
                        var match = Regex.Match(decodeEdilmis, "src=[\"'](.*?)[\"']", RegexOptions.IgnoreCase);
                        if (match.Success)
                        {
                            resimUrl = match.Groups[1].Value;
                        }
                    }

                    if (resimUrl.StartsWith("//")) resimUrl = "https:" + resimUrl;
                    if (resimUrl.StartsWith("http:")) resimUrl = resimUrl.Replace("http:", "https:");

                    string temizAciklama = Regex.Replace(aciklama ?? "", "<.*?>", string.Empty).Trim();
                    temizAciklama = System.Net.WebUtility.HtmlDecode(temizAciklama);

                    haberler.Add(new Haber
                    {
                        Baslik = baslik,
                        Link = link,
                        Ozet = temizAciklama,
                        Tarih = tarih,
                        Resim = resimUrl.Trim()
                    });
                }
                cvHaberler.ItemsSource = haberler;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Hata", $"Bağlantı sorunu: {ex.Message}", "Tamam");
            }
            finally
            {
                loading.IsRunning = false;
                loading.IsVisible = false;
            }
        }

        private async void CvHaberler_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var secilen = e.CurrentSelection.FirstOrDefault() as Haber;
            if (secilen == null) return;
            cvHaberler.SelectedItem = null;

            var navigationParameter = new Dictionary<string, object> { { "HaberObj", secilen } };
            await Shell.Current.GoToAsync(nameof(HaberDetayPage), navigationParameter);
        }
    }
}