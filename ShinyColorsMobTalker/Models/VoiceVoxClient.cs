using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace ShinyColorsMobTalker.Models
{
    static class VoiceVoxClient
    {
        private static  string baseUrl = "http://127.0.0.1";

        private static  int port = 50021;

        private static HttpClient client = new HttpClient();


        public static async Task<string> GetQuery(string text)
        {
            string url = $"{baseUrl}:{port}/audio_query";
            int speaker = 1;
            var parameters = new Dictionary<string, string>()
            {
                {"text", text },
                {"speaker", speaker.ToString()}
            };

            url = url + "?" + await new FormUrlEncodedContent(parameters).ReadAsStringAsync();
            HttpContent content = new StringContent("");
            HttpResponseMessage response = await client.PostAsync(url, content);
            string query = await response.Content.ReadAsStringAsync();
            return query;
        }


        public static async void Speek(string query)
        {
            string url = $"{baseUrl}:{port}/synthesis";
            int speaker = 1;
            var parameters = new Dictionary<string, string>()
            {
                {"speaker", speaker.ToString()}
            };
            url = url + "?" + await new FormUrlEncodedContent(parameters).ReadAsStringAsync();

            var content = new StringContent(query, Encoding.UTF8, @"application/json");

            HttpResponseMessage response = await client.PostAsync(url, content);

            using(var stream = await response.Content.ReadAsStreamAsync())
            {
                var player = new SoundPlayer(stream);
                player.PlaySync();
            }
        }

    }
}
