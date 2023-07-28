using Ovh.Api;
using System.Net;
using System.Text.Json;
using WGOvh.Models;
using WGOvh.Services;
using WGOvh.Settings;

public class DomainService
{
	private readonly Ovh.Api.Client _client;
	private readonly OvhSettings ovhSettings;
	private readonly ILogger<WorkerService> _logger;

	public DomainService(ILogger<WorkerService> logger, IConfiguration configuration)
	{
		_logger = logger;
		ovhSettings = OvhSettings.GetSettings(configuration);
		_client = new Client(ovhSettings.ApiUrl, ovhSettings.ApplicationKey, ovhSettings.ApplicationSecret, ovhSettings.ConsumerKey);
	}

	public async Task<string[]> GetDomains() {
		var domains = await _client.GetAsync<string[]>("/domain/zone");
		return domains;
	}
	public static string GetFullDomain(RecordDNS recordDNS) {
		var domain = string.Empty; if (recordDNS == null) return domain;
		if (!string.IsNullOrEmpty(recordDNS.zone)) { domain = recordDNS.zone; }
		if (!string.IsNullOrEmpty(recordDNS.subDomain)) domain = $"{recordDNS.subDomain}.{domain}";
		return domain;
	}
	public bool ToBeUpdated(string defaultDomain)
	{
		var defaultDomainExist = false;
		if (ovhSettings.Domains == null || ovhSettings.Domains.Count == 0) return defaultDomainExist;
		foreach (var domain in ovhSettings.Domains)
		{
			if (domain == null) continue;
			if (domain.ToUpper().Equals(defaultDomain.ToUpper()))
			{
				defaultDomainExist = true;
				break;
			}
		}
		return defaultDomainExist;
	}

	public async Task<RecordDNS> GetDnsRecord(string domainName, long dsnRecordId) {
		var result =  await _client.GetAsync<RecordDNS>($"/domain/zone/{domainName}/record/{dsnRecordId}");
		return result;
	} 

	public async Task<long[]> GetDomainDNSRecords(string domain) {
		var domainIds = await _client.GetAsync<long[]>($"/domain/zone/{domain}/record?fieldType=A");
		return domainIds;
	}

	public async Task<RecordDNS> PutRecordDNS(string domain, long recordId, RecordDNS recordDNS) {
		var serialitedRecord = JsonSerializer.Serialize(recordDNS);
		await _client.PutAsync($"/domain/zone/{domain}/record/{recordId}", recordDNS);
		return await  GetDnsRecord(domain, recordId);
	}

	public async Task UpdateDomains(IPAddress publicIp) {
		var domains = await GetDomains();
		foreach (var domain in domains)
		{

			if (!ToBeUpdated(domain)) continue;
			var domainRecordsId = await GetDomainDNSRecords(domain);
			if (domainRecordsId == null) continue;

			foreach (var recordId in domainRecordsId) {
				try 
				{
					var domainrecord = await GetDnsRecord(domain, recordId);
					if (domainrecord == null) continue;
					if (domain.ToUpper() != GetFullDomain(domainrecord).ToUpper()) continue;
					if (domainrecord.target == publicIp.ToString()) continue;
					domainrecord.target = publicIp.ToString();
					_logger.LogInformation($"Updating domain {domain} to ip {publicIp.ToString()}");
					await PutRecordDNS(domain, recordId, domainrecord);
					_logger.LogInformation($"Domain {domain} updated to ip {publicIp.ToString()}");

				} catch (Exception e) {					
					_logger.LogError(e.Message, e);
				}
			}

		}
	}

}