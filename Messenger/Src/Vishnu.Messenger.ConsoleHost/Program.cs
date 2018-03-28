using Microsoft.Owin.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vishnu.Messenger.ConsoleHost
{
    class Program
    {
        static void Main(string[] args)
        {
            string baseAddress = "http://localhost:10060/";
            using (WebApp.Start(baseAddress))
            {
                Console.WriteLine("Vishnu Messaging Server hosted on " + baseAddress);
                Console.WriteLine("Press any key to Stop");
                Console.ReadLine();
            }
        }
    }
}
