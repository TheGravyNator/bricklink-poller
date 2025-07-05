using System.Text;
using System.Text.Json;

namespace BrickLinkPoller.Services
{
	public class DiscordMessager
	{
		HttpClient _httpClient;
		Configuration config;

		string webhookUrl;

		public DiscordMessager()
		{
			_httpClient = new HttpClient();
			config = new Configuration();

			webhookUrl = config.configuration["Settings:Webhook"]!;
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
