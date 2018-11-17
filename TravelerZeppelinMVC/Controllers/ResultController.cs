using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TravelerZeppelinMVC.Utilities;
using TravelerZeppelinMVC.Types;

namespace TravelerZeppelinMVC.Controllers
{
    public class ResultController : Controller
    {
        // GET: Result
        public ActionResult ShowResult(string destination)
        {
            ViewBag.Destination = destination;
            string destinationPlate = Utilities.Utilities.cities.FirstOrDefault(x => x.CityName == destination).Plate;
            List<string> subPlates = Utilities.Utilities.nodes.FirstOrDefault(x => x.Plate == destinationPlate).SubPlates;
            List<City> subCities = new List<City>();
            foreach (var item in subPlates)
            {
                City subCity = Utilities.Utilities.cities.FirstOrDefault(x => x.Plate == item);
                subCities.Add(subCity);
            }
            City destinationCity = Utilities.Utilities.cities.FirstOrDefault(x => x.CityName == destination);
            subCities.Add(destinationCity);
            ViewBag.SubCities = subCities;
            return View();
        }

        public ActionResult First(string destination)
        {
            string destinationPlate = Utilities.Utilities.cities.FirstOrDefault(x => x.CityName == destination).Plate;
            List<string> subPlates= Utilities.Utilities.nodes.FirstOrDefault(x => x.Plate == destinationPlate).SubPlates;
            List<City> subCities = new List<City>();
            foreach (var item in subPlates)
            {
                City subCity = Utilities.Utilities.cities.FirstOrDefault(x => x.Plate == item);
                subCities.Add(subCity);
            }
            return Json(subCities,JsonRequestBehavior.AllowGet);
        }

    }
}