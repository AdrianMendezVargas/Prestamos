using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Prestamos.API.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Prestamos.API.Controllers.HttpClientHelpers {
    public class RequestHelper {

        static HttpClient httpClient = new HttpClient() {
            BaseAddress = new Uri("https://localhost:44379/api")
        };

        public static async Task<T> GetAsync<T>(string requestUri) {
            var response = await httpClient.GetAsync($"{httpClient.BaseAddress}/{requestUri}");
            var Json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(Json);
        }

        public static async Task<HttpResponseMessage> PostAsync<T>(string requestUri, T item) {
            var Json = JsonConvert.SerializeObject(item);
            var buffer = System.Text.Encoding.UTF8.GetBytes(Json);
            var byteContent = new ByteArrayContent(buffer);

            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            return await httpClient.PostAsync($"{httpClient.BaseAddress}/{requestUri}" , byteContent);
        }

    }
}
