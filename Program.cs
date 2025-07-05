using Microsoft.Extensions.Configuration;

namespace BrickLinkPoller
{
    internal class Program
    {
		static DiscordMessager messager;

        static async Task Main(string[] args)
        {
            var messageString = "Ohayo Sekai!";

			messager = new DiscordMessager();
            await messager.SendMessage(messageString);

			Console.WriteLine(messageString);
        }
    }
}
