using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Odev3.Models
{
    public class Gorev
    {
        public string Id { get; set; }          
       public string Baslik { get; set; }      
       public string Detay { get; set; }       
        public bool YapildiMi { get; set; }     
       public DateTime Tarih { get; set; }     
   }
}
