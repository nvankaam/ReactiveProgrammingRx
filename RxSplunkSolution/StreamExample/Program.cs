using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Repo;
using System.Net;

namespace StreamExample
{
    class Program
    {
        public static void Main(string[] args)
        {

            var file = new RepoApacheLogLine().LogLines.ToObservable();
            var ipCmp = IPAddress.Parse("80.101.212.178");

            
            var count = 0;

            var res = file;

            using (res.Subscribe(
                logs => {
                    Console.WriteLine(logs);
                    count++;
                },
                error => {
                    
                }, () => {Console.WriteLine(count++);}
            ))

            Console.ReadLine();

        }

    }
}
