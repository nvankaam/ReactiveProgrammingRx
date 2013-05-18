using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Repo;

namespace StreamExample
{
    class Program
    {
        public static void Main(string[] args)
        {

            var file = new FileOpener().LogFileLines.ToObservable();

            var res = file.Where(o => o.Length < 100);

            var count = 0;

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
