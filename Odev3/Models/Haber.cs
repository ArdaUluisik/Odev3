using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Odev3.Models;
namespace Odev3.Models
{
    public class Haber
    {
        public string Baslik { get; set; }
        public string Ozet { get; set; }
        public string Link { get; set; }
        public string Resim { get; set; }
        public string Tarih { get; set; }
    }

public class RssRoot
    {
        public List<Haber> items { get; set; }
    }
}






