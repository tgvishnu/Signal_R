using Microsoft.Owin.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Vishnu.Messenger.ApiManagement.ConsoleHost
{
    class Program
    {
        static void Main(string[] args)
        {
            string baseAddress = "http://localhost:10050/";
            using (WebApp.Start<Startup>(url: baseAddress))
            {
                Console.WriteLine("User administration WEB API hosted on " + baseAddress);
                Console.WriteLine("Press any key to Stop");
                Console.ReadLine();
            }
        }
    }
}
