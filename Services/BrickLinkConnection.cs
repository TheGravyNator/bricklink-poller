using BrickLinkPoller.Models;
using RestSharp;
using RestSharp.Authenticators;
using RestSharp.Authenticators.OAuth;
using System.Text.Json;

namespace BrickLinkPoller.Services
{
	public class BrickLinkConnection
	{
		Configuration config;
		RestClient client;
		JsonSerializerOptions options;

        string[] excludedStatuses = 
		{
			"PURGED",
			"COMPLETED",
			"CANCELLED"
		};

		public BrickLinkConnection()
		{
			config = new Configuration();

			var consumerKey = config.configuration["OAuth:ConsumerKey"];
			var consumerSecret = config.configuration["OAuth:ConsumerSecret"];
			var token = config.configuration["OAuth:Token"];
			var tokenSecret = config.configuration["OAuth:TokenSecret"];

			var authenticator = OAuth1Authenticator.ForProtectedResource(
				consumerKey!,
				consumerSecret!,
				token!,
				tokenSecret!,
				OAuthSignatureMethod.HmacSha1
			);

			client = new RestClient(new RestClientOptions("https://api.bricklink.com/api/store/v1/")
			{
				Authenticator = authenticator
			});

			options = new JsonSerializerOptions
			{
				PropertyNameCaseInsensitive = true
			};
			options.Converters.Add(new BrickLinkOrderConverter());
		}

		public async Task<List<BrickLinkOrder>?> GetOrders()
		{
			var request = new RestRequest("orders", Method.Get);
			request.AddQueryParameter("direction", "out");

			var response = await client.GetAsync(request);

			if (!response.IsSuccessful)
			{
				Console.WriteLine($"Error: {response.StatusCode} - {response.Content}");
				throw new Exception(response.ErrorMessage);
			}

			try
			{
				var brickLinkResponse = JsonSerializer.Deserialize<BrickLinkOrderResponse>(
					response.Content!, 
					new JsonSerializerOptions
					{
						PropertyNameCaseInsensitive = true
					});

                if (brickLinkResponse != null)
				{
                    var detailedResponses = new List<BrickLinkOrder>();
                    foreach (var order in brickLinkResponse.Data.Where(order => !excludedStatuses.Contains(order.Status)).ToList())
                    {
                        var detailedOrder = await GetOrder(order.Order_Id);
                        if (detailedOrder != null)
                        {
                            detailedResponses.Add(detailedOrder);
                        }
                    }

					return detailedResponses;
						
				}
				else
				{
					return null;
				}
			}
			catch (Exception ex) 
			{
				throw new Exception($"Could not deserialize the orders due to: {ex.Message}");
			}
		}

        public async Task<BrickLinkOrder?> GetOrder(int orderId)
        {
            var request = new RestRequest($"orders/{orderId}", Method.Get);
            request.AddQueryParameter("direction", "out");

            var response = await client.GetAsync(request);

            if (!response.IsSuccessful)
            {
                Console.WriteLine($"Error: {response.StatusCode} - {response.Content}");
                throw new Exception(response.ErrorMessage);
            }

            try
            {
                var brickLinkResponse = JsonSerializer.Deserialize<BrickLinkOrderSingleResponse>(response.Content!, options);

                if (brickLinkResponse != null)
                {
					return brickLinkResponse.Data;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Could not deserialize the order due to: {ex.Message}");
            }
        }
    }
}
