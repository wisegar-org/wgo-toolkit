using Spectre.Console;
using System.Net;
using System.Text.Json;
using WGOvh.Models;
using WGOvh.Settings;

namespace WGOvh.Services
{
	public static class SettingsService
	{
		private const string AppSettingsFileName = "appsettings.json";
		public static AppSettingsModel GetSettingsFile()
		{

			try
			{
				using var sr = new StreamReader(AppSettingsFileName);
				var settingsFile = JsonSerializer.Deserialize<AppSettingsModel>(sr.ReadToEnd());
				if (settingsFile != null)
				{
					return settingsFile;
				}

			}
			catch (IOException e)
			{
				Console.WriteLine("The settings file could not be read:");
				Console.WriteLine(e.Message);
			}

			return new AppSettingsModel();
		}

		public static async Task SaveAppSettingsAsync(AppSettingsModel settings) {
			using FileStream createStream = File.Create(AppSettingsFileName);
			await JsonSerializer.SerializeAsync(createStream, settings);
			await createStream.DisposeAsync();
		}

		public static async Task<IPAddress?> GetPublicIp(string serviceUrl = "https://checkip.amazonaws.com")
		{
			//"http://icanhazip.com"
			var externalIpString = (await new HttpClient().GetStringAsync(serviceUrl));
			var parsedExternalId = externalIpString.Replace("\\r\\n", "").Replace("\\n", "").Trim();
			if (!IPAddress.TryParse(parsedExternalId, out var ipAddress)) return null;
			return ipAddress;
		}

		public static bool IsDomainValid(this string domain)
		{
			if (string.IsNullOrEmpty(domain)) return false;
			if (!domain.Contains('.')) return false;
			var domainSplitted = domain.Split('.');
			if (domainSplitted.Length != 2) return false;
			var domainName = domainSplitted[0];
			if (string.IsNullOrEmpty(domainName)) return false;
			var domainExt = domainSplitted[1];
			if (string.IsNullOrEmpty(domainExt) || domainExt.Length < 2) return false;
			return true;
		}
		public static bool IsSubDomainValid(this string subDomain)
		{
			if (string.IsNullOrEmpty(subDomain)) return false;
			if (!subDomain.Contains('.')) return false;
			var domainSplitted = subDomain.Split('.');
			if (domainSplitted.Length != 3) return false;
			var subDomainName = domainSplitted[0];
			if (string.IsNullOrEmpty(subDomainName)) return false;
			var domainName = domainSplitted[1];
			if (string.IsNullOrEmpty(domainName)) return false;
			var domainExt = domainSplitted[2];
			if (string.IsNullOrEmpty(domainExt) || domainExt.Length < 2) return false;
			return true;

		}

		private static void GetInputDomainSettings(AppSettingsModel settingsFile)
		{
			if (settingsFile.OvhSettings.Domains == null || settingsFile.OvhSettings.Domains.Count == 0)
			{
				settingsFile.OvhSettings.Domains = new List<string>();
			}

			AnsiConsole.WriteLine("Insert the domain to be used. Press 'q' and enter to close the input mode!");
			int index = 0;
			while (true)
			{
				var defaultValue = settingsFile.OvhSettings.Domains.Count > index && settingsFile.OvhSettings.Domains[index] != null ? settingsFile.OvhSettings.Domains[index] : "";
				var askResult = AnsiConsole.Ask($"{index + 1} :", defaultValue);
				index++;
				if (askResult.IsDomainValid())
				{
					settingsFile.OvhSettings.Domains.Add(askResult);
					continue;
				}
				else
				{
					index--;
				}
				if (askResult.ToUpper() == "Q") break;
			}
		}

		private static void GetInputSubDomainSettings(AppSettingsModel settingsFile) {

			if (settingsFile.OvhSettings.SubDomains == null || settingsFile.OvhSettings.SubDomains.Count == 0)
			{
				settingsFile.OvhSettings.SubDomains = new List<string>();
			}

			AnsiConsole.WriteLine("Insert the subdomains to be used. Press 'q' and enter to close the input mode!");
			var index = 0;
			while (true)
			{
				var defaultValue = settingsFile.OvhSettings.SubDomains.Count > index && settingsFile.OvhSettings.SubDomains[index] != null ? settingsFile.OvhSettings.SubDomains[index] : "";
				var askResult = AnsiConsole.Ask($"{index + 1} :", defaultValue);
				index++;
				if (askResult.IsSubDomainValid())
				{
					settingsFile.OvhSettings.SubDomains.Add(askResult);
					continue;
				}
				else
				{
					index--;
				}
				if (askResult.ToUpper() == "Q") break;
			}
		}

		public static async Task GetInputSettingsAsync() {

			AppSettingsModel settingsFile = GetSettingsFile();
			settingsFile.OvhSettings ??= new OvhSettings();
			settingsFile.OvhSettings.ApiUrl = AnsiConsole.Ask<string>("Insert the ovh region endpoint", "ovh-eu");
			settingsFile.OvhSettings.ApplicationKey = AnsiConsole.Ask<string>("Insert the ovh application key", settingsFile.OvhSettings.ApplicationKey);
			settingsFile.OvhSettings.ApplicationSecret = AnsiConsole.Ask<string>("Insert the ovh application secret", settingsFile.OvhSettings.ApplicationSecret);
			settingsFile.OvhSettings.ConsumerKey = AnsiConsole.Ask<string>("Insert the ovh consumerKey", settingsFile.OvhSettings.ConsumerKey);

			GetInputDomainSettings(settingsFile);

			GetInputSubDomainSettings(settingsFile);

			await SaveAppSettingsAsync(settingsFile);
		}
	}
}
