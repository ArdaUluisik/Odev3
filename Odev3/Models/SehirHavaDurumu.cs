namespace Odev3.Models
{
   
    public class GunlukTahmin
    {
        public string GunIsmi { get; set; }   
        public string Tarih { get; set; }     
        public string Sicaklik { get; set; }  
        public string Resim { get; set; }     
        public string Durum { get; set; }     
    }

    
    public class WeatherResponse
    {
        public DailyUnits daily_units { get; set; }
        public DailyData daily { get; set; }
    }

    public class DailyUnits
    {
        public string temperature_2m_max { get; set; }
    }

    public class DailyData
    {
        public string[] time { get; set; }               
        public int[] weathercode { get; set; }           
        public double[] temperature_2m_max { get; set; } 
        public double[] temperature_2m_min { get; set; } 
    }
    public class GeoResponse
    {
        public List<GeoLocation> results { get; set; }
    }

    public class GeoLocation
    {
        public string name { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
    }
}