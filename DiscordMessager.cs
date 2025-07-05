using System.Text;
using System.Text.Json;

namespace BrickLinkPoller
{
	public class DiscordMessager
	{
		HttpClient _httpClient;
		string webhookUrl;

		public DiscordMessager() 
		{
			_httpClient = new HttpClient();
			webhookUrl = "https://discord.com/api/webhooks/1391000751953674321/9wPnBmWDoraPSofaZWyyD2yXmc7wKXZOYmRpZnMkjt3aIgH1Ce_3lMoM79OIogTUZn8U";
		}

		public async Task SendMessage(string message)
		{
			var payload = new
			{
				content = message
			};

			var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

			var response = await _httpClient.PostAsync(webhookUrl, content);

			if (response.IsSuccessStatusCode)
			{
				Console.WriteLine("Message sent successfully!");
			}
			else
			{
				Console.WriteLine($"Failed to send message. Status code: {response.StatusCode}");
				string responseContent = await response.Content.ReadAsStringAsync();
				Console.WriteLine($"Response: {responseContent}");
			}
		}
	}
}
