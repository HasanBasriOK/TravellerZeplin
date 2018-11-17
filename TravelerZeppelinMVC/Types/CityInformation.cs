using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TravelerZeppelinMVC.Types
{
    public class CityInformation
    {
        public string StartCityPlate { get; set; }
        public double DistanceValue { get; set; }
        public string CityPlate { get; set; }
        public List<string> Plates { get; set; }
    }
}