using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repo
{
    public class FileOpener
    {
        public String[] LogFileLines{ get; set; }
        public FileOpener()
        {
            LogFileLines = System.IO.File.ReadAllLines("Files/access_log.txt");
        }
    }
}
