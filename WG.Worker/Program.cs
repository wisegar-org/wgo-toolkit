using Spectre.Console;
using WGOvh.Services;

	
AnsiConsole.MarkupLine($"[underline springgreen3_1 bold]WGO WORKER[/]");
Console.WriteLine();


SettingsService.PrintSettings();
Console.WriteLine();

AnsiConsole.WriteLine("To edit settings run command using option: -s");
Console.WriteLine();

if (args.Length > 0 && args[0] == "-s")
{
	var clearSettings = AnsiConsole.Confirm("Do you want to clear the settings", false);

	if (clearSettings)
	{
		await SettingsService.CleanSettingsAsync();
		Console.WriteLine();
		SettingsService.PrintSettings();
		Console.WriteLine();
	}

	var updateSettings = AnsiConsole.Confirm("Do you want to update the settings", false);

	if (updateSettings)
	{
		await SettingsService.GetInputSettingsAsync();
		Console.WriteLine();
	}
}



IHost host = AppBuilder.Build(args)
	.ConfigureServices((hostContext, services) =>
	{
		services.AddHostedService<WorkerService>();
	})
	.Build();

await host.RunAsync();


