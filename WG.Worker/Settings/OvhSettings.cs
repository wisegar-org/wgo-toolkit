namespace WGOvh.Settings
{
	public class OvhSettings
	{
		public string ApiUrl { get; set; } = string.Empty;
		public string ApplicationKey { get; set; } = string.Empty;
		public string ApplicationSecret { get; set; }
		= string.Empty;
		public string ConsumerKey { get; set; } = string.Empty;
		public List<string> Domains { get; set; } = new List<string>();
		public List<string> SubDomains { get; set; } = new List<string>();

		public static OvhSettings GetSettings(IConfiguration configuration) {
			return configuration.GetSection(nameof(OvhSettings))
												   .Get<OvhSettings>();
		}
	}
}
