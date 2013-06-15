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
	}
}
