using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Net.Http;
using System.Web;
using Microsoft.Azure.Devices;
using System.Globalization;
using System.Net;
using Microsoft.Azure.Devices.Common;
using PCLCrypto;

namespace Sec_DiscoServer.Controller
{
    public class TokenController : ApiController
    {
        [Route("IoTHub/GetToken")]
        [HttpGet]
        public async Task<string> Get()
        {
            //Device Id - IoTHub Connection Information
            string deviceId = "<<Replace with ID of your Device. - E.g.: Device01;>>";
            string ioTHubName = "<<Replace with Name of your IoT Hub instance>>.azure-devices.net - E.g.: robertsiothub.azure-devices.net";
            string ioTHubConnectionString = "<<Replace with IoT Hub Connection String from Azure Portal - E.g.:HostName=robertsiothub.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=BZ1...Q=";

            //Get Device Symmetric Key
            RegistryManager registryManager = RegistryManager.CreateFromConnectionString(ioTHubConnectionString);
            Device device = await registryManager.GetDeviceAsync(deviceId);
            string deviceSymmetricKey = device.Authentication.SymmetricKey.PrimaryKey;

            //Create Token with fixed Endtime - 31st of December 2016
            DateTime _epochTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            DateTime endLiveTime = new DateTime(2016,12,31);
            
            TimeSpan secondsEpochTime = endLiveTime.Subtract(_epochTime);
            long seconds = Convert.ToInt64(secondsEpochTime.TotalSeconds, CultureInfo.InvariantCulture);
            string expiresOnSeconds = Convert.ToString(seconds, CultureInfo.InvariantCulture);

            string url = WebUtility.UrlEncode("{0}/devices/{1}".FormatInvariant(ioTHubName, deviceId));
            string signature = Sign($"{url}\n{expiresOnSeconds}" , deviceSymmetricKey);

            StringBuilder sharedAccessSignature = new StringBuilder();
            sharedAccessSignature.AppendFormat(CultureInfo.InvariantCulture, "{0} {1}={2}&{3}={4}&{5}={6}",
                "SharedAccessSignature",
                "sr", url,
                "sig", WebUtility.UrlEncode(signature),
                "se", WebUtility.UrlEncode(expiresOnSeconds));

            return sharedAccessSignature.ToString();
        }

        static string Sign(string requestString, string key)
        {
            var algorithm = WinRTCrypto.MacAlgorithmProvider.OpenAlgorithm(MacAlgorithm.HmacSha256);
            var hash = algorithm.CreateHash(Convert.FromBase64String(key));
            hash.Append(Encoding.UTF8.GetBytes(requestString));
            var mac = hash.GetValueAndReset();
            return Convert.ToBase64String(mac);
        }

    }
}
