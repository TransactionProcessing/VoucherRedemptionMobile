namespace VoucherRedemptionMobile.Services
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using ClientProxyBase;
    using Common;
    using Database.Entities;
    using Newtonsoft.Json;

    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="ClientProxyBase" />
    /// <seealso cref="IConfigurationServiceClient" />
    public class ConfigurationServiceClient : ClientProxyBase, IConfigurationServiceClient
    {
        /// <summary>
        /// The base address resolver
        /// </summary>
        private readonly Func<String, String> BaseAddressResolver;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationServiceClient"/> class.
        /// </summary>
        /// <param name="baseAddressResolver">The base address resolver.</param>
        /// <param name="httpClient">The HTTP client.</param>
        public ConfigurationServiceClient(Func<String, String> baseAddressResolver,
                                          HttpClient httpClient) : base(httpClient)
        {
            this.BaseAddressResolver = baseAddressResolver;
        }

        /// <summary>
        /// Builds the request URL.
        /// </summary>
        /// <param name="route">The route.</param>
        /// <returns></returns>
        private String BuildRequestUrl(String route)
        {
            String baseAddress = this.BaseAddressResolver("ConfigServiceUrl");

            String requestUri = $"{baseAddress}{route}";

            return requestUri;
        }

        /// <summary>
        /// Gets the configuration.
        /// </summary>
        /// <param name="deviceIdentifier">The device identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public async Task<Configuration> GetConfiguration(String deviceIdentifier,
                                                          CancellationToken cancellationToken)
        {
            Console.WriteLine($"Getting config for device [{deviceIdentifier}]");
            Configuration response = null;
            String requestUri = this.BuildRequestUrl($"/voucherconfiguration/{deviceIdentifier}");

            Console.WriteLine($"requestUri: {requestUri}");
            try
            {
                // Make the Http Call here
                HttpResponseMessage httpResponse = await this.HttpClient.GetAsync(requestUri, cancellationToken);
                    
                // Process the response
                String content = await this.HandleResponse(httpResponse, cancellationToken);

                Console.WriteLine($"content: {content}");

                // call was successful so now deserialise the body to the response object
                response = JsonConvert.DeserializeObject<Configuration>(content);
            }
            catch (Exception ex)
            {
                // An exception has occurred, add some additional information to the message
                Exception exception = new Exception($"Error getting configuration for device Id {deviceIdentifier}.", ex);

                throw exception;
            }

            return response;
        }

        /// <summary>
        /// Posts the diagnostic logs.
        /// </summary>
        /// <param name="deviceIdentifier">The device identifier.</param>
        /// <param name="logMessages">The log messages.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task PostDiagnosticLogs(String deviceIdentifier, 
                                             List<LogMessage> logMessages,
                                             CancellationToken cancellationToken)
        {
            String requestUri = this.BuildRequestUrl($"/logging/{deviceIdentifier}");

            // Create a container
            var container = new
                            {
                                messages = logMessages
                            };
            StringContent content = new StringContent(JsonConvert.SerializeObject(container), Encoding.UTF8, "application/json");
            
            HttpResponseMessage httpResponse = await this.HttpClient.PostAsync(requestUri, content, cancellationToken);

            await this.HandleResponse(httpResponse, cancellationToken);
        }

        /// <summary>
        /// Handles the response.
        /// </summary>
        /// <param name="responseMessage">The response message.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        protected override async Task<String> HandleResponse(HttpResponseMessage responseMessage,
                                                             CancellationToken cancellationToken)
        {
            String content = await responseMessage.Content.ReadAsStringAsync();

            if (responseMessage.StatusCode == HttpStatusCode.NotFound)
            {
                // No error as maybe running under CI (which has no internet)
                return content;
            }
            
            return await base.HandleResponse(responseMessage, cancellationToken);
        }
    }
}