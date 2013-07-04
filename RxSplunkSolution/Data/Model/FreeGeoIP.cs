using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Reactive.Threading;
using System.Reactive.Concurrency;
using System.Xml.Linq;

namespace Data.Repo
{

    public class Response
    {
        public string Ip { get; set; }
        public string Countrycode { get; set; }
        public string CountryName { get; set; }
        public string RegionCode { get; set; }
        public string RegionName { get; set; }
        public string City { get; set; }
        public string ZipCode { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string MetroCode { get; set; }
        public string AreaCode { get; set; }
    }

    public class FreeGeoIP
    {

        public GeoIp getIPLocation(String IP)
        {
            var client = new System.Net.WebClient();

            string downloadedString = client.DownloadString("http://freegeoip.net/xml/"+IP);

            XmlSerializer mySerializer =
                new XmlSerializer(typeof(Response));

            Response response = null;

            XmlReader xmlReader = XmlReader.Create(new System.IO.StringReader(downloadedString));

            response = (Response)mySerializer.Deserialize(xmlReader);

            return new GeoIp()
            {
                Ip = IP,
                Lat = Convert.ToDouble(response.Latitude),
                Long = Convert.ToDouble(response.Longitude)
            };

        }

        public GeoIp getIPLocation2(String IP)
        {

            XmlDocument foo = new XmlDocument();

            foo.Load(String.Format("http://freegeoip.net/xml/{0}", IP));

            XmlNode root = foo.DocumentElement;

            XmlNode latitude = root.SelectSingleNode("/Response/Latitude");
            XmlNode longitude = root.SelectSingleNode("/Response/Longitude");

            return new GeoIp()
            {
                Ip = IP,
                Lat = Convert.ToDouble(latitude.InnerText),
                Long = Convert.ToDouble(longitude.InnerText)
            };

        }


        public IObservable<GeoIp> getIPLocation3(String IP)
        {
            WebClient wc = new WebClient();
            var result = wc.DownloadStringTaskAsync(new Uri("http://freegeoip.net/xml/" + IP)).ToObservable();
           return result.Select(x =>
            {
                XDocument xDoc = XDocument.Load(x);
                var mydata = (from item in xDoc.Root.Elements("Response")
                             select new GeoIp()
                             {
                                 Ip = IP,
                                 Lat = Convert.ToDouble((string)item.Element("Latitude")),
                                 Long = Convert.ToDouble((string)item.Element("Longitude"))
                             }).Single();
                return mydata;
            });
        }

    }

   
}
