// Set up configuration sources.
using Microsoft.Extensions.Configuration;
using static System.Runtime.InteropServices.JavaScript.JSType;

var builder = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true);

IConfiguration config = builder.Build();

var service = new AssistantsSdkPoc.AssistantService(config);
(string assistantId, byte[] data) = await service.RunAssistantAsync();
var fileName = config["FilePath"];

await File.WriteAllBytesAsync(fileName, data);

await service.DeleteAssistantAsync(assistantId);