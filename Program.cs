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

var workflow = new List<FileCommand>
{
    new FileCommand(sourcePath, Path.Combine(archivePath, fileName), FileOperation.Copy),
    new FileCommand(Path.Combine(archivePath, fileName), Path.Combine(destinationPath, fileName), FileOperation.Move),
    new FileCommand(sourcePath, "", FileOperation.Delete)
};

foreach (var command in workflow)
{
    command.Execute(logger);
}

logger.LogInformation("FileFerry workflow completed.");
