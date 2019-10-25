using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using VaultSharp;
using VaultSharp.V1.AuthMethods;
using VaultSharp.V1.AuthMethods.Token;

namespace VaultClientDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            //            SdkSecretWriterV2();
            //            SdkSecretReaderV2();

            SdkSecretReaderV1();
            SdkSecretWriterV1();
            SdkSecretReaderV1();

            //            HttpWriteValue().Wait();
            //            HttpReadValue().Wait();
            Console.ReadLine();
        }

        private static void SdkSecretReaderV2()
        {
            IAuthMethodInfo authMethod = new TokenAuthMethodInfo(vaultToken: "s.NfKF7cpNmpvy3o76BaiY5d7O");
            var vaultClientSettings = new VaultClientSettings("http://127.0.0.1:8200", authMethod)
            {
                ContinueAsyncTasksOnCapturedContext = false
            };

            var vaultClient = new VaultClient(vaultClientSettings);

            Dictionary<string, object> secrets = vaultClient.V1.Secrets.KeyValue.V2
                .ReadSecretAsync(mountPoint: "secret", path: "hello").Result.Data.Data;

            foreach (var keyValuePair in secrets)
            {
                Console.WriteLine($"[{keyValuePair.Key}]=[{keyValuePair.Value}]");
            }

            Console.WriteLine(secrets);
        }

        private static void SdkSecretReaderV1()
        {
            IAuthMethodInfo authMethod = new TokenAuthMethodInfo(vaultToken: "s.4jsklbJUaQuMKmgEESZYLuWS");
            var vaultClientSettings = new VaultClientSettings("http://127.0.0.1:8200", authMethod)
            {
                ContinueAsyncTasksOnCapturedContext = false
            };

            var vaultClient = new VaultClient(vaultClientSettings);

            Dictionary<string, object> secrets = vaultClient.V1.Secrets.KeyValue.V1
                .ReadSecretAsync(mountPoint: "edge", path: "/password/password/userlogging/secretkey").Result.Data;

            foreach (var keyValuePair in secrets)
            {
                Console.WriteLine($"[{keyValuePair.Key}]=[{keyValuePair.Value}]");
            }

            Console.WriteLine(secrets);
        }


        private static void SdkSecretWriterV1()
        {
            IAuthMethodInfo authMethod = new TokenAuthMethodInfo(vaultToken: "s.4jsklbJUaQuMKmgEESZYLuWS");
            var vaultClientSettings = new VaultClientSettings("http://127.0.0.1:8200", authMethod)
            {
                ContinueAsyncTasksOnCapturedContext = false
            };

            var vaultClient = new VaultClient(vaultClientSettings);

            vaultClient.V1.Secrets.KeyValue.V1
                .WriteSecretAsync(mountPoint:"edge", path: "/password/password/userlogging/secretkey", values: new Dictionary<string, object>
                {
                    {
                        "/password/password/userlogging/secretkey", "there"
                    }
                }).Wait();

        }


        private static async Task HttpReadValue()
        {
            try
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri("http://127.0.0.1:8200/v1/");
                client.DefaultRequestHeaders.Add("X-Vault-Token", "s.NfKF7cpNmpvy3o76BaiY5d7O");


                HttpResponseMessage response = await client.GetAsync("secret/data/hello");

                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                var json = (JObject) JsonConvert.DeserializeObject(responseBody);
                Console.WriteLine(responseBody);

            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
            }
        }

        public class JsonContent : StringContent
        {
            public JsonContent(object obj) :
                base(JsonConvert.SerializeObject(obj), Encoding.UTF8, "application/json")
            { }
        }

        private static async Task HttpWriteValue()
        {
            try
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri("http://127.0.0.1:8200/v1/");
                client.DefaultRequestHeaders.Add("X-Vault-Token", "s.NfKF7cpNmpvy3o76BaiY5d7O");
                var data =
                    new
                    {
                        data = new Dictionary<string, string>
                        {
                            {"foo", "bar"},
                            {"zip", "zap1"}
                        }
                    };
                
                HttpContent content = new StringContent(JsonConvert.SerializeObject(data));
                content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");


                //mount/data/:path
                HttpResponseMessage response = await client.PostAsync("secret/data/hello", content);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                var json = (JObject)JsonConvert.DeserializeObject(responseBody);
                Console.WriteLine(responseBody);
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
            }

        }
    }
}
