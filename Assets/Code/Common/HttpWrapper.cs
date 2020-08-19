using System;
using System.Net.Http;
using System.Threading.Tasks;
using Plugins.submodules.SharedCode.Logger;

namespace Code.Common
{
    public static class HttpWrapper
    {
        private static readonly ILog Log = LogManager.CreateLogger(typeof(HttpWrapper));
        private static readonly HttpClient HttpClient = new HttpClient();
        
        public static async Task<byte[]> Post(string url, (string name, string value)[] fields)
        {
            Log.Info("Отправка post запроса по url = " + url);
            try
            {
                using (var formData = new MultipartFormDataContent())
                {
                    foreach ((string name, string value) in fields)
                    {
                        HttpContent stringContent = new StringContent(value);
                        formData.Add(stringContent, name);
                    }
                    var response = await HttpClient.PostAsync(url, formData);
                    if (!response.IsSuccessStatusCode)
                    {
                        throw new Exception("Unexpected status code = " + response.StatusCode);
                    }
                    string base64String = await response.Content.ReadAsStringAsync();
                    byte[] data = Convert.FromBase64String(base64String);
                    return data;
                }
            }
            catch(Exception e)
            {
                Log.Warn("Упало при отправке/обработке post запроса " + e.Message + " " + e.InnerException);
                throw;
            }
        }
        
        public static async Task<byte[]> Get(string url, (string name, string value)[] query)
        {
            Log.Info("Отправка get запроса по url = " + url);
            try
            {
                url += "?";
                foreach ((string name, string value) in query)
                {
                    url += name + "=" + value+"&";
                }
                var response = await HttpClient.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception("Unexpected status code = " + response.StatusCode);
                }
                string base64String = await response.Content.ReadAsStringAsync();
                byte[] data = Convert.FromBase64String(base64String);
                return data;
            }
            catch(Exception e)
            {
                Log.Warn("Упало при отправке/обработке get запроса " + e.Message + " " + e.InnerException);
                throw;
            }
        }
    }
}