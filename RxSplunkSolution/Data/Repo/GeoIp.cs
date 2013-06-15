using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repo
{
	public class GeoIp
	{
		public int GeoIpId { get; set; }
		public string Ip { get; set; }
		public long Long { get; set; }
		public long Lat { get; set; }
	}
}
