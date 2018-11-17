using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TravelerZeppelinMVC.Types;

namespace TravelerZeppelinMVC.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            Utilities.Utilities.nodes = new List<Types.Node>();
            Utilities.Utilities.FillNodes();
            ViewBag.Cities = Utilities.Utilities.cities;
            return View();
        }
        //yüzde elli kar problemi
        public ActionResult FiftyPercentProfit(string source,string destination,int peopleCount)
        {
            int[] yolcuSayisi = new int[] { 10, 20, 30, 40, 50 };

            string sourcePlate = Utilities.Utilities.cities.FirstOrDefault(x => x.CityName == source).Plate;
            string destinationPlate = Utilities.Utilities.cities.FirstOrDefault(x => x.CityName == destination).Plate;

            int kmMaliyet = 10;

            Utilities.Utilities.nodes = new List<Node>();
            Utilities.Utilities.maxInclination = 80 - peopleCount;
            Utilities.Utilities.UseDijkstra(sourcePlate, true, string.Empty);
            double destinationValue = Utilities.Utilities.nodes.FirstOrDefault(x => x.Plate == destinationPlate).Value;

            double maliyet = destinationValue * kmMaliyet;
            double olmasiGerekenKar = maliyet / 2;
            double priceOfOnePerson = (maliyet + olmasiGerekenKar) / peopleCount;

            ProfitCalculater result= new ProfitCalculater
            {
                Node = Utilities.Utilities.nodes.FirstOrDefault(x => x.Plate == destinationPlate),
                PeopleCount = peopleCount,
                PriceOfOnePerson = priceOfOnePerson
            };

            return Json(result,JsonRequestBehavior.AllowGet);
        }
        //sabit ücret problemi
        public ActionResult StaticPrice(string source, string destination)
        {
            string sourcePlate = Utilities.Utilities.cities.FirstOrDefault(x => x.CityName == source).Plate;
            string destinationPlate = Utilities.Utilities.cities.FirstOrDefault(x => x.CityName == destination).Plate;

            int kmMaliyet = 10;
            int staticPrice = 100;

            List<StaticPriceProblem> staticPriceResults = new List<StaticPriceProblem>();

            for (int i = 5; i < 51; i++)
            {
                Utilities.Utilities.nodes = new List<Node>();
                Utilities.Utilities.maxInclination = 80 - i;
                Utilities.Utilities.UseDijkstra(sourcePlate, true, string.Empty);

                double destinationValue = Utilities.Utilities.nodes.FirstOrDefault(x => x.Plate == destinationPlate).Value;

                double maliyet = destinationValue * kmMaliyet;
                double price = i * staticPrice;
                double profit = price - maliyet;

                double profitPercent = (100 * profit) / maliyet;

                staticPriceResults.Add(new StaticPriceProblem { Node = Utilities.Utilities.nodes.FirstOrDefault(x => x.Plate == destinationPlate),PeopleCount=i,Profit=profitPercent });
            }

            StaticPriceProblem result = staticPriceResults.OrderByDescending(x => x.Profit).First();

            return Json(result,JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult ShowOnMap(string destination)
        {
            return RedirectToAction("ShowResult", "Result", new { Destination = destination });
        }
    }
}