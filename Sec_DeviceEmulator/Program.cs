using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Sec_DeviceEmulator
{
    class Program
    {
        static void Main(string[] args)
        {

            //Device Id - IoTHub Connection Information
            string deviceId = "<<Replace with ID of your Device. - E.g.: Device01;>>";
            string ioTHubName = "<<Replace with Name of your IoT Hub instance>>.azure-devices.net - E.g.: robertsiothub.azure-devices.net";

            //Get Token from Discovery Service
            //Url from Discovery Service - Sync with Url from Sec_DiscoServer/Program.cs
            string discoveryServiceUrl = "http://localhost:8080/IoTHub/GetToken";
            string sASToken = "";
            using (HttpClient httpClient = new HttpClient())
            {
                Task<HttpResponseMessage> getTokenTask = httpClient.GetAsync(discoveryServiceUrl);
                
                Task<string> readTokenTask = getTokenTask.Result.Content.ReadAsStringAsync();
                sASToken = readTokenTask.Result.Trim('"'); 
            }

            //Ingest to IoT Hub - http Post - Use Token from Discovery Service
            Uri targetUri = new Uri($"https://{ioTHubName}/devices/{deviceId}/messages/events?api-version=2015-08-15-preview");
            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
                httpClient.DefaultRequestHeaders.Add("Authorization", sASToken);
                HttpContent httpContent = new StringContent("Payload");
                Task<HttpResponseMessage> task = httpClient.PostAsync(targetUri, httpContent);
                if (task.Result.IsSuccessStatusCode)
                    Console.WriteLine("Successful Telemetry Ingest");
                else
                    Console.WriteLine($"Http Status Code: {task.Result.StatusCode}");
            }

            Console.WriteLine("Press any key ...");
            Console.ReadLine();

        }
    }
}
