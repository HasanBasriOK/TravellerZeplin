using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using TravelerZeppelinMVC.Types;

namespace TravelerZeppelinMVC.Utilities
{
    public class Utilities
    {

        public static List<City> cities = new List<City>();
        public static List<CityInformation> informations = new List<CityInformation>();
        public static List<City> controlledCities = new List<City>();
        public static List<Node> nodes = new List<Node>();
        public static string startCityPlate = String.Empty;
        public static List<SubNode> subNodes = new List<SubNode>();
        public static int maxInclination;

        public Utilities()
        {
           
        }

        public static void UseDijkstra(string plate, bool isFirst,string comingCityPlate)
        {
            if (nodes.Count == 0)
                FillNodes();

            //başlangıç şehrinin değeri sıfır olarak set edilir
            if (isFirst)
            {
                nodes.FirstOrDefault(x => x.Plate == plate).Value = 0;
                nodes.FirstOrDefault(x => x.Plate == plate).IsCompleted = true;
                startCityPlate = plate;
            }


            //komşu şehirler arasında dolanılarak o komşulara olan uzaklık ile değer set edilir.
            foreach (var neighbour in cities.FirstOrDefault(x => x.Plate == plate).Neighbours)
            {
                if (neighbour.Plate!=comingCityPlate && neighbour.Plate!=startCityPlate)
                {
                    double latitude1 = cities.FirstOrDefault(x => x.Plate == plate).Latitude;
                    double latitude2 = cities.FirstOrDefault(x => x.Plate == neighbour.Plate).Latitude;
                    double longtitude1= cities.FirstOrDefault(x => x.Plate == plate).Longtitude;
                    double longtitude2 = cities.FirstOrDefault(x => x.Plate == neighbour.Plate).Longtitude;
                    double altitude1 = cities.FirstOrDefault(x => x.Plate == plate).Altitude;
                    double altitude2 = cities.FirstOrDefault(x => x.Plate == neighbour.Plate).Altitude;

                    double distance1 = GetDistance(latitude1,latitude2,longtitude1,longtitude2,altitude1,altitude2);
                    double distanceHorizontal = GetDistanceHorizontal(latitude1, latitude2, longtitude1, longtitude2);
                    double inclination = GetInclination(altitude1, altitude2, 50, distanceHorizontal);
                    //hesaplanan eğim maximum eğimden kçük veya eşit ise gidilebilir.
                    //Diğer türlü o komşuya gidilemez
                    if (inclination<=maxInclination)
                    {
                        double distance2 = nodes.FirstOrDefault(x => x.Plate == plate).Value;
                        double sumDistance = distance1 + distance2;
                        //komşunun değeri daha önce bir başka şehir üzerinden bulunmuş mu kontrol edilir.
                        //bulunmuş ise yeni değer eski değerden küçük mü diye bakılır ve küçük ise yeni değer set edilir.
                        //yeni değer küçük değil ise güncelleme yapılmaz.
                        if (nodes.FirstOrDefault(x => x.IsCompleted != null && x.Plate == neighbour.Plate) != null)
                        {
                            //yeni değer daha küçük ise nodes listesi içindeki değer değiştirilir.
                            //yeni değer değiştirildiği için izlenilen yol da değiştirilir.
                            if (nodes.FirstOrDefault(x => x.IsCompleted != null && x.Plate == neighbour.Plate).Value > sumDistance)
                            {
                                nodes.FirstOrDefault(x => x.IsCompleted != null && x.Plate == neighbour.Plate).Value = sumDistance;
                                nodes.FirstOrDefault(x => x.IsCompleted != null && x.Plate == neighbour.Plate).SubPlates.Clear();
                                nodes.FirstOrDefault(x => x.IsCompleted != null && x.Plate == neighbour.Plate).SubPlates.AddRange(nodes.FirstOrDefault(x => x.Plate == plate).SubPlates);
                                nodes.FirstOrDefault(x => x.IsCompleted != null && x.Plate == neighbour.Plate).SubPlates.Add(plate);
                            }
                        }
                        else
                        {
                            nodes.FirstOrDefault(x => x.Plate == neighbour.Plate).Value = sumDistance;
                            //komşuları arasında dolandığımız şehir başlangıç noktası değil ise ona giderken izlenecek yola önceki yollar da eklenir.
                            if (!isFirst)
                            {
                                nodes.FirstOrDefault(x => x.Plate == neighbour.Plate).SubPlates.AddRange(nodes.FirstOrDefault(x => x.Plate == plate).SubPlates);
                            }
                            nodes.FirstOrDefault(x => x.Plate == neighbour.Plate).SubPlates.Add(plate);
                            nodes.FirstOrDefault(x => x.Plate == neighbour.Plate).IsCompleted = false;
                        }
                    }
                }
            }
            nodes.FirstOrDefault(x => x.Plate == plate).IsCompleted = true;

            Node node = nodes.FirstOrDefault(x => x.IsCompleted == false);
            if (node!=null)
            {
                UseDijkstra(node.Plate,false,plate);
            }
        }

        public static double GetDistanceHorizontal(double latitude1, double latitude2, double longtitude1, double longtitude2)
        {
            double teta = longtitude1 > longtitude2 ? longtitude1 - longtitude2 : longtitude2 - longtitude1;
            double mile = Math.Sin(DegreeToRadian(latitude1)) * Math.Sin(DegreeToRadian(latitude2)) + Math.Cos(DegreeToRadian(latitude1)) * Math.Cos(DegreeToRadian(latitude2)) * Math.Cos(DegreeToRadian(teta));

            mile = Math.Acos(mile);
            mile = RadianToDegree(mile);
            mile = mile * 60 * 1.1515;

            double kilometer = mile * 1.609344;

            return kilometer;
        }

        public static double GetDistance(double latitude1,double latitude2,double longtitude1,double longtitude2,double altitude1,double altitude2)
        {
            double teta = longtitude1 > longtitude2 ? longtitude1 - longtitude2:longtitude2-longtitude1;
            double mile = Math.Sin(DegreeToRadian(latitude1)) * Math.Sin(DegreeToRadian(latitude2)) + Math.Cos(DegreeToRadian(latitude1)) * Math.Cos(DegreeToRadian(latitude2)) * Math.Cos(DegreeToRadian(teta));

            mile = Math.Acos(mile);
            mile = RadianToDegree(mile);
            mile = mile * 60 * 1.1515;

            double kilometer = mile * 1.609344;

            double altitudeDifferenceByMeter = altitude2 > altitude1 ? altitude2 - altitude1 : altitude1 - altitude2;
            double altitudeDifferenceByKilometer;
            double distance;
            if (altitudeDifferenceByMeter != 0)
            {
                altitudeDifferenceByKilometer = altitudeDifferenceByMeter / 1000;
                distance = Math.Sqrt((kilometer * kilometer) + (altitudeDifferenceByKilometer * altitudeDifferenceByKilometer));
            }
            else
            {
                distance = kilometer;
            }
            return distance;
            //return kilometer;
        }

        public static double DegreeToRadian(double degree)
        {
            return Convert.ToDouble(degree * Math.PI / 180);
        }

        public static double RadianToDegree(double radian)
        {
            return Convert.ToDouble(radian / Math.PI * 180);
        }
        
        public static double GetInclination(double altitude1,double altitude2,double staticHeight,double distance)
        {
            double differenceAltitude = altitude1 > altitude2 ? altitude1 - altitude2 : altitude2 - altitude1;
            double inclination = Math.Atan(differenceAltitude + staticHeight/distance) ;
            return inclination;
        }

        private static void SetNeighbours()
        {
            string komsuluklarPath = ConfigurationManager.AppSettings["KomsuluklarPath"].ToString();
            using (var fs = new FileStream(komsuluklarPath, FileMode.Open))
            {
                var file = new StreamReader(fs, Encoding.UTF8);

                string lineText = string.Empty;
                while ((lineText = file.ReadLine()) != null)
                {
                    lineText = lineText.Replace("\t", "").Replace("\r", "");
                    string[] values = lineText.Split(',');
                    string mainCityPlate = values[0];

                    for (int i = 1; i < values.Length; i++)
                    {

                        if (!string.IsNullOrEmpty(values[i]))
                        {
                            City neigbour = new City();
                            City orjinalCity = cities.FirstOrDefault(x => x.Plate == values[i]);
                            neigbour.Plate = orjinalCity.Plate;
                            neigbour.Longtitude = orjinalCity.Longtitude;
                            neigbour.Latitude = orjinalCity.Latitude;
                            neigbour.Altitude = orjinalCity.Altitude;
                            neigbour.CityName = orjinalCity.CityName;
                            if (cities.FirstOrDefault(x => x.Plate == mainCityPlate).Neighbours == null)
                                cities.FirstOrDefault(x => x.Plate == mainCityPlate).Neighbours = new List<City>();

                            cities.FirstOrDefault(x => x.Plate == mainCityPlate).Neighbours.Add(neigbour);
                        }
                    }
                }
            }
        }

        public static void FillNodes()
        {

            string cityInformationPath = ConfigurationManager.AppSettings["CityInformationPath"].ToString();
            using (var fs = new FileStream(cityInformationPath, FileMode.Open))
            {
                var file = new StreamReader(fs, Encoding.UTF8);
                cities = Newtonsoft.Json.JsonConvert.DeserializeObject<List<City>>(file.ReadToEnd());

                SetNeighbours();

                foreach (var city in cities)
                {
                    nodes.Add(new Types.Node
                    {
                        Plate = city.Plate,
                        Value = 0,
                        IsCompleted = null,
                        SubPlates = new List<string>()
                    });
                }
            }

        }
    }
}