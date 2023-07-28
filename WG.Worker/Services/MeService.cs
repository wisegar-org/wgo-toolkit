using Ovh.Api;
using WGOvh.Models;
using WGOvh.Settings;

namespace WGOvh.Services
{
	public class MeService
	{
		private readonly Ovh.Api.Client client;

		public MeService(OvhSettings settings) {
			client = new Client(settings.ApiUrl, settings.ApplicationKey, settings.ApplicationSecret, settings.ConsumerKey);
		}

		public async Task<PartialMe> MeAsync()
		{
			return await this.client.GetAsync<PartialMe>("/me");
		}

	}
}
