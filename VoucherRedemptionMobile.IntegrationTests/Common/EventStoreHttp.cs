using System;
using System.Collections.Generic;
using System.Text;

namespace VoucherRedemptionMobile.IntegrationTests.Common
{
    using System.IO;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading;
    using System.Threading.Tasks;

    public class EventStoreHttp
    {
        private readonly IPEndPoint EventStoreHttpEndpoint;

        public EventStoreHttp(IPEndPoint eventStoreHttpEndpoint)
        {
            this.EventStoreHttpEndpoint = eventStoreHttpEndpoint;
        }

        public async Task CreateContinuousProjection(String name,
                                               String query,
                                               Boolean trackEmittedStreams,
                                               String username,
                                               String password,
                                               CancellationToken cancellationToken)
        {
            // Build up the stream name
            String url = this.EventStoreHttpEndpoint.ToHttpUrl("https", "/projections/continuous?name={0}&type=JS&emit=1&trackemittedstreams={1}", name, trackEmittedStreams);

            // Post the projection
            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, url);

            // Build the authentication value
            String httpAuthentication = string.Format("{0}:{1}", username, password);
            String encodedCredentials = Convert.ToBase64String(Encoding.UTF8.GetBytes(httpAuthentication));
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Basic", encodedCredentials);

            // Now build the body
            Byte[] bodyBytes = Encoding.UTF8.GetBytes(query);
            MemoryStream stream = new MemoryStream(bodyBytes);
            StreamContent content = new StreamContent(stream);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            content.Headers.ContentLength = bodyBytes.Length;

            requestMessage.Content = content;

            HttpResponseMessage response = await this.PostProjection(requestMessage, cancellationToken);

            if (response.StatusCode == HttpStatusCode.Created)
            {
                // All good, just return
                return;
            }
            else if (response.StatusCode == HttpStatusCode.Conflict)
            {
                throw new Exception($"Projection Command Conflict Error ({response.StatusCode})");
            }
            else
            {
                throw new Exception($"Projection Command Failed Error ({response.StatusCode})");
            }

        }

        private async Task<HttpResponseMessage> PostProjection(HttpRequestMessage requestMessage, CancellationToken cancellationToken)
        {
            HttpClientHandler clientHandler = new HttpClientHandler();
            clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };

            using (HttpClient client = new HttpClient(clientHandler))
            {
                return await client.SendAsync(requestMessage, cancellationToken);
            }
        }
    }

    public static class IPEndpointExtensions
    {

        public static string ToHttpUrl(this IPEndPoint endPoint, string schema, string formatString,
                                       params object[] args)
        {
            return CreateHttpUrl(schema, endPoint.ToString(), string.Format(formatString.TrimStart('/'), args));
        }

        private static string CreateHttpUrl(string schema, string address, string path)
        {
            return $"{schema}://{address}/{path}";
        }
    }
}
