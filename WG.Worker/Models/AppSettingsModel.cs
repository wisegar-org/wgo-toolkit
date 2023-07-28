using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Xml;
using WGOvh.Settings;

namespace WGOvh.Models
{
    public class AppSettingsModel
    {
        public Logging Logging { get; set; } = new Logging();
        public OvhSettings OvhSettings { get; set; } = new OvhSettings();
    }
    public class Logging
    {
        public LogLevel LogLevel { get; set; } = new LogLevel();
    }

    public class LogLevel
    {
        public string Default { get; set; } = "Information";
        [JsonPropertyName("Microsoft.Hosting.Lifetime")]
        public string LifeTimeHosting { get; set; } = "Information";

    }

}
