public class AppBuilder
{
    public static IHostBuilder Build(string[] args)
    {
        var version = Environment.OSVersion.VersionString;
        if (version.ToLower().Contains("windows"))
            return BuildWin(args);
        return BuildLinux(args);
    }
    public static IHostBuilder BuildWin(string[] args)
    {
        return Host.CreateDefaultBuilder(args);
    }
    public static IHostBuilder BuildLinux(string[] args)
    {
        return Host.CreateDefaultBuilder(args).UseSystemd();
    }
}