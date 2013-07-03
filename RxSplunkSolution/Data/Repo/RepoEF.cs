using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace Data.Repo
{
	public class RepoEF : DbContext
	{
		public DbSet<GeoIp> GeoIps { get; set; }

        public async Task<IEnumerable<GeoIp>> getAsync(string ip)
        {
			//List<GeoIp> geo = await (from b in GeoIps
			//			where b.Ip.Equals(ip)
			//			select b).ToListAsync();
            /*
            var query = from b in GeoIps
                        where b.Lat == Lat && b.Long == Long
                        select b;*/

          //  return geo;
			return null;
        }

        public GeoIp get(string ip)
        {
            var geo = from b in GeoIps
                      where b.Ip.Equals(ip)
                      select b;

            return geo.First();
        }
	}
}
