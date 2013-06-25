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

        public async Task<IEnumerable<GeoIp>> get(long Lat, long Long)
        {
            List<GeoIp> geo = await (from b in GeoIps
                        where b.Lat == Lat && b.Long == Long
                        select b).ToListAsync();
            /*
            var query = from b in GeoIps
                        where b.Lat == Lat && b.Long == Long
                        select b;*/

            return geo;
        }
	}
}
