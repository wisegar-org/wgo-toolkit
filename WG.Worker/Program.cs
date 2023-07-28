using Spectre.Console;
using WGOvh.Services;

AnsiConsole.MarkupLine($"[underline springgreen3_1 bold]WGO WORKER[/]");

await SettingsService.GetInputSettingsAsync();


IHost host = AppBuilder.Build(args)
	.ConfigureServices((hostContext, services) =>
	{
		services.AddHostedService<WorkerService>();
	})
	.Build();

await host.RunAsync();


