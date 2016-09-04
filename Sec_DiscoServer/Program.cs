using Microsoft.Azure.Devices;
using Microsoft.Owin.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Sec_DiscoServer
{
    class Program
    {
        static void Main(string[] args)
        {
            //**************************************************
            //* Please modify/change IoT Hub - Device Settings 
            //* in Sec_DiscoServer/Controller/TokenController.cs
            //**************************************************
            
            //Self-Hosted REST API; Local Port No
            string baseAddress = "http://localhost:8080/";

            using (WebApp.Start<Startup>(baseAddress))
            {
                Console.ReadLine();
            }

        }
    }
}
