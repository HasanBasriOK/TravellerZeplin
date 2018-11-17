using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TravelerZeppelinMVC.Types
{
    public class City
    {
        public string CityName { get; set; }
        public double Altitude { get; set; }
        public double Latitude { get; set; }
        public double Longtitude { get; set; }
        public string Plate { get; set; }
        public List<City> Neighbours { get; set; }
    }
}