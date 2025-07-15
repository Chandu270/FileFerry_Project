using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System.IO;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((context, config) =>
    {
        config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
    })
    .ConfigureLogging(logging =>
    {
        logging.ClearProviders();
        logging.AddConsole();
    })
    .Build();

var logger = host.Services.GetRequiredService<ILogger<Program>>();
var configRoot = host.Services.GetRequiredService<IConfiguration>();

var sourcePath = configRoot["Paths:Source"];
var archivePath = configRoot["Paths:Archive"];
var destinationPath = configRoot["Paths:Destination"];
var fileName = Path.GetFileName(sourcePath);

var watcher = new FileSystemWatcher
{
    Path = sourcePath,
    Filter = "*.*",
    EnableRaisingEvents = true,
    NotifyFilter = NotifyFilters.FileName | NotifyFilters.CreationTime
};

watcher.Created += (s, e) =>
{
    try
    {
        logger.LogInformation($"New file detected: {e.FullPath}");
        var fileName = Path.GetFileName(e.FullPath);

        var workflow = new List<FileCommand>
        {
            new FileCommand(e.FullPath, Path.Combine(archivePath, fileName), FileOperation.Copy),
            new FileCommand(Path.Combine(archivePath, fileName), Path.Combine(destinationPath, fileName), FileOperation.Move),
            new FileCommand(e.FullPath, "", FileOperation.Delete)
        };

        foreach (var command in workflow)
        {
            command.Execute(logger);
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error processing file event.");
    }
};

logger.LogInformation("FileFerry workflow completed.");
