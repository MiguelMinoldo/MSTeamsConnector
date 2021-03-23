using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MSTeamsConnector.Models;
using Newtonsoft.Json;

namespace MSTeamsConnector.Services
{
    public class FunctionService : IFunctionService
    {
        public static void SerializeJsonIntoStream(object value, Stream stream)
        {
            using (var stremw = new StreamWriter(stream, new UTF8Encoding(false), 1024, true))
            using (var jsonw = new JsonTextWriter(stremw) { Formatting = Formatting.None })
            {
                var js = new JsonSerializer();

                js.Serialize(jsonw, value);
                jsonw.Flush();
            }
        }

        private static HttpContent CreateHttpContent(object content)
        {
            HttpContent httpContent = null;

            if (content != null)
            {
                var memorystream = new MemoryStream();

                SerializeJsonIntoStream(content, memorystream);

                memorystream.Seek(0, SeekOrigin.Begin);
                httpContent = new StreamContent(memorystream);
                httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            }

            return httpContent;
        }
        public async Task<IActionResult> DoPostAsync(TeamsMessage teamsMessage)
        {
            var Url = "https://msteamsconnector.azurewebsites.net/api/teams/send?code=arh/PyoH7ODyknpHU/BgK28aw9vKI63OdH//Msku/hQWJ77zA6QszQ==";

            CancellationToken cancellationToken;

            using (var client = new HttpClient())
            using (var request = new HttpRequestMessage(HttpMethod.Post, Url))
            using (var httpContent = CreateHttpContent(teamsMessage))
            {
                request.Content = httpContent;

                using (var newresponse = await client
                    .SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken)
                    .ConfigureAwait(false))
                {

                    var result = newresponse.Content.ReadAsAsync<IActionResult>();

                    return result.Result;
                }
            }
        }
    }
}