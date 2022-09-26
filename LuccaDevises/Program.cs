using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using LuccaDevises.Abstractions;
using LuccaDevises.Services;

class Program
{
    static void Main(string[] args)
    {
        Host.CreateDefaultBuilder(args)
            .ConfigureServices(ConfigureServices)
            .Build()
            .Services
            .GetService<ILuccaDevisesService>()?
            .Execute(args);
    }


    private static void ConfigureServices(HostBuilderContext hostContext, IServiceCollection services)
    {
            services.AddSingleton<IFileService, FileService>()
                .AddSingleton<IProgramArgumentValidationService, ProgramArgumentValidationService>()
                .AddSingleton<IExchangeRequestValidationService, ExchangeRequestValidationService>()
                .AddSingleton<IExchangeRequestService, ExchangeRequestService>()
                .AddSingleton<IExchangeRequestService, ExchangeRequestService>()
                .AddSingleton<IOutputService, OutputService>()
                .AddSingleton<ILuccaDevisesService, LuccaDevisesService>();
    }
}