using Microsoft.Extensions.Configuration;

namespace BrickLinkPoller
{
	public class Configuration
	{
		public IConfigurationRoot configuration; 

		public Configuration() 
		{
			var builder = new ConfigurationBuilder()
				.AddJsonFile("appsettings.json", optional: true)
				.AddUserSecrets<Program>();

			configuration = builder.Build();
		}
	}
}
