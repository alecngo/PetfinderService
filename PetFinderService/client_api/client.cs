using System.Net.Http.Headers;
using PetFinderService;

namespace client_api
{
    public partial class Client 
    {
        private readonly HttpClient _httpClient;
        private static Client? _existingClient;
        private static readonly object _lock = new object();
        private static Exception? _clientError;

        private string AuthorizationToken = "Bearer";
        private const string DefaultBaseURL = "https://api.petfinder.com/v2";

        private Client() 
        {
            _httpClient = new HttpClient();
        }

        /// Gets or initialises a client.
        /// @return A reference to the newly initialized client instance or an existing client
        public static Client GetClient() 
        {
            Logger.Info("GetClient() called.");

            /// Initialize the client. If the client is already initialized it will be created.
            if (_existingClient == null) 
            {
                lock (_lock) 
                {
                    /// Initialize the client. If the client already exists, it will refer to that one. 
                    if (_existingClient == null) 
                    {
                        try 
                        {
                            Logger.Info("Attempting to initialize client.");

                            string? clientID = System.Environment.GetEnvironmentVariable("PF_CLIENT_ID");
                            string? clientSecret = System.Environment.GetEnvironmentVariable("PF_CLIENT_SECRET");

                            if (string.IsNullOrEmpty(clientID) || string.IsNullOrEmpty(clientSecret))
                            {
                                Logger.Info("Error: PF_CLIENT_ID and/or PF_CLIENT_SECRET are missing.");
                                Logger.Info("Please ensure you have set both the PF_CLIENT_ID and PF_CLIENT_SECRET environment variables.");
                                throw new InvalidOperationException("Client ID and/or Client Secret not set in environment variables.");
                            }

                            _existingClient = new Client();
                            _existingClient.InitializeAsync(clientID, clientSecret).GetAwaiter().GetResult();
                        }

                        catch (Exception ex) 
                        {
                            Logger.Info($"Error initializing client: {ex.Message}");
                            _clientError = ex;
                        }
                    }
                }
            }

            if (_clientError != null) 
            {
                throw _clientError;
            }

            if (_existingClient == null) 
            {
                throw new InvalidOperationException("Client has not been initialized.");
            }

            Logger.Info("Returning existing client instance.");
            return _existingClient;
        }

        /// Gets an access token from 
        /// 
        /// @param clientId - The client_id of the client that is requesting the access token.
        /// @param clientSecret - The client_secret of the client that is requesting the access token.
        /// 
        /// @return A string that can be used to make requests to the newtonsoft API with the token returned as part of the response
        private async Task<string> GetAccessTokenAsync(string clientId, string clientSecret)
        {
            var tokenRequest = new HttpRequestMessage(HttpMethod.Post, $"{GetURL()}/oauth2/token");
            tokenRequest.Content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["grant_type"] = "client_credentials",
                ["client_id"] = clientId,
                ["client_secret"] = clientSecret
            });

            var response = await _httpClient.SendAsync(tokenRequest);
            
            var responseBody = await response.Content.ReadAsStringAsync();
            Logger.Info($"Token request response: {responseBody}");  // Log the entire response for debugging

            /// If the access token is not successful throw an exception.
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Failed to fetch access token. Status: {response.StatusCode}. Body: {responseBody}");
            }

            var tokenData = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(responseBody);
            /// Returns the access token from the response.
            if (tokenData == null || tokenData?.access_token == null)
            {
                throw new InvalidOperationException("Failed to obtain a valid access token from the response.");
            }

            return tokenData!.access_token;        
        }

        /// Initializes the client with access token.
        /// 
        /// @param clientId - The client id of the application. Can be null if the application doesn't have a client.
        /// @param clientSecret - The client secret of the application. Can be null if the application doesn't have a client
        private async Task InitializeAsync(string? clientId, string? clientSecret) {
            try {
                string accessToken = await GetAccessTokenAsync(clientId!, clientSecret!);
                Logger.Info($"Retrieved Access Token: {accessToken}");
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(AuthorizationToken, accessToken);
                Logger.Info($"Authorization Header Set: {_httpClient.DefaultRequestHeaders.Authorization}");
                Logger.Info($"Fetched access token: {accessToken}");
            } 
            catch (Exception ex) 
            {
                Logger.Info($"Error fetching access token: {ex.Message}");
                throw;
            }
        }

        /// Refreshes the client state.
        public void RefreshClient()
        {
            lock (_lock)
            {
                Logger.Info("Refreshing client...");

                // Clean up existing client state
                _httpClient.DefaultRequestHeaders.Clear();

                // Re-initialize
                string? clientID = System.Environment.GetEnvironmentVariable("PF_CLIENT_ID");
                string? clientSecret = System.Environment.GetEnvironmentVariable("PF_CLIENT_SECRET");
                InitializeAsync(clientID, clientSecret).GetAwaiter().GetResult();
            }
        }

        /// Gets the PF_BASE_URL environment variable if set otherwise falls back to the default.
        private static string GetURL() 
        {
            string? url = System.Environment.GetEnvironmentVariable("PF_BASE_URL");
            if (!string.IsNullOrEmpty(url)) 
            {
                return url;
            }
            return DefaultBaseURL;
        }

        /// Asynchronously performs a HTTP GET request to the specified URL. The response is returned as a byte array
        public async Task<byte[]> HttpGetAsync(string url) 
        {
            Logger.Info($"Sending GET request to: {url}");

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);
            HttpResponseMessage response = await _httpClient.SendAsync(request);

            /// If the response is a success status code and the response body contains JSON or plain text error details.
            if (!response.IsSuccessStatusCode) 
            {
                string errorMsg = $"Request failed with status code {response.StatusCode}: {response.ReasonPhrase}";
                
                // If the response body contains JSON or plain text error details
                // Append it to the errorMsg
                /// If the response is not a JSON or text plain message
                if (response.Content.Headers.ContentType?.MediaType == "application/json" || 
                    response.Content.Headers.ContentType?.MediaType == "text/plain") {
                    errorMsg += $"\n{await response.Content.ReadAsStringAsync()}";
                }

                Logger.Info($"Error in GET request: {errorMsg}");
                throw new HttpRequestException(errorMsg);
            }

            Logger.Info($"GET request successful. Status code: {response.StatusCode}");
            byte[] body = await response.Content.ReadAsByteArrayAsync();
            return body;
        }

        /// Sends a GET request to Aysnc and returns the response.
        public async Task<byte[]> SendGetRequestAysnc(string path) 
        {
            Logger.Info($"Preparing to send GET request with path: {path}");
            string url = $"{GetURL()}{path}";

            byte[] body = await HttpGetAsync(url);

            Logger.Info("GET request completed and data received.");
            return body;
        }
    }
}
