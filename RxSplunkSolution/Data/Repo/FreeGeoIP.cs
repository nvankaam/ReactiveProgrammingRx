using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

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

        public String getIPLocation(String IP)
        {
            var client = new System.Net.WebClient();

            string downloadedString = client.DownloadString("http://freegeoip.net/xml/"+IP);

            XmlSerializer mySerializer =
                new XmlSerializer(typeof(Response));

            Response response = null;

            XmlReader xmlReader = XmlReader.Create(new System.IO.StringReader(downloadedString));

            response = (Response)mySerializer.Deserialize(xmlReader);
            return "Location: " + response.Latitude + ":" + response.Longitude;

        }

        public String getIPLocation2(String IP)
        {

            XmlDocument foo = new XmlDocument();

            //Let's assume that the IP of the target player is in args[1]
            //This allows us to parameterize the Load method to reflect the IP address
            //of the user per the OP's request
            foo.Load(String.Format("http://freegeoip.net/xml/{0}", IP));

            XmlNode root = foo.DocumentElement;

            // you might need to tweak the XPath query below
            XmlNode latitude = root.SelectSingleNode("/Response/Latitude");
            XmlNode longitude = root.SelectSingleNode("/Response/Longitude");

            return "Location: " + latitude.InnerText + ":" + longitude.InnerText;

        }
    }

   
}
