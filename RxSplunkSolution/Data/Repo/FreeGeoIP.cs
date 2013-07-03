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
using System.IO;
using System.Reflection;

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

        public IObservable<GeoIp> getGeoLocation(String IP)
        {
            WebClient wc = new WebClient();
            var result = wc.DownloadStringTaskAsync(new Uri("http://freegeoip.net/xml/" + IP)).ToObservable();
            return result.Select(x =>
            {
                XDocument xDoc = XDocument.Parse(x);
                var res = new GeoIp()
                    {
                        Ip = IP,
                        Lat = Convert.ToDouble((string)xDoc.Root.Element("Latitude")),
                        Long = Convert.ToDouble((string)xDoc.Root.Element("Longitude"))
                    };
                return res;
            });
        }
    }
}
