using Microsoft.Extensions.Configuration;

namespace BrickLinkPoller
{
    internal class Program
    {
		static DiscordMessager messager;

        static async Task Main(string[] args)
        {
            var messageString = "Ohayo Sekai!";

			var builder = new ConfigurationBuilder()
				.AddJsonFile("appsettings.json", optional: false)
				.AddUserSecrets<Program>(); 

			var configuration = builder.Build();

			var webhook = configuration["Settings:Webhook"];

			messager = new DiscordMessager();
            await messager.SendMessage(messageString);

			Console.WriteLine(messageString);
        }
    }
}
