using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Shared;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;

namespace ManualConnectedCiotola
{
    class Program
    {
        static async Task Main(string[] args)
        {
            int _dosi;

            var builder = new ConfigurationBuilder()
              .SetBasePath(Directory.GetCurrentDirectory())
              .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            var config = builder.Build();
            var deviceId = config["deviceId"];
            var authenticationMethod = new DeviceAuthenticationWithRegistrySymmetricKey(deviceId, config["key"]);
            var transportType = TransportType.Mqtt_WebSocket_Only;
            
            var client = DeviceClient.Create(deviceId, authenticationMethod, transportType);


            while (true)
            {
                Message receivedMessage = await client.ReceiveAsync();
                if (receivedMessage == null)
                {
                    continue;
                } else
                {
                    var message = Encoding.UTF8.GetString(receivedMessage.GetBytes());

                    if(message == "Feed")
                    {
                        var twin = await client.GetTwinAsync();
                        if (twin.Properties.Reported.Contains("dosi"))
                        {
                            _dosi = (int)twin.Properties.Reported["dosi"];
                            if (_dosi > 0)
                            {
                                _dosi--;
                            }
                        } else
                        {
                            _dosi = 10;
                        }

                        var nTwin = new TwinCollection();
                        nTwin["dosi"] = _dosi;
                        await client.UpdateReportedPropertiesAsync(nTwin);

                    }
                    else if (message == "Foto")
                    {
                        var connectionString = config["StorageConnectionString"];
                        var storageAccount = CloudStorageAccount.Parse(connectionString);
                        var blobClient = storageAccount.CreateCloudBlobClient();
                        var imageContainer = blobClient.GetContainerReference("images");
                        var blob = imageContainer.GetBlockBlobReference("image.jpg");
                        await blob.UploadFromFileAsync("image.jpg");
                        var blobUrl = blob.StorageUri.PrimaryUri.ToString();

                        var nTwin = new TwinCollection();
                        nTwin["imageUrl"] = blobUrl;
                        await client.UpdateReportedPropertiesAsync(nTwin);
                    }
                }

            }
        }
    }
}
