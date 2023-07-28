using System.Text.Json.Serialization;

namespace WGOvh.Models
{
	public class RecordDNS
	{
		[JsonPropertyName("id")]
        public long id { get; set; }
		
		[JsonPropertyName("zone")]
		public string? zone { get; set; }

		[JsonPropertyName("subDomain")]
		public string? subDomain { get; set; }

		[JsonPropertyName("fieldType")]
		public string? fieldType { get; set; }

		[JsonPropertyName("target")]
		public string? target { get; set; }

		[JsonPropertyName("ttl")]
		public int ttl { get; set; }
	}
}
