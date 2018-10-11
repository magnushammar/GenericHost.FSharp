module GenericHostSample

open System
open System.Threading.Tasks
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.Logging
open System.IO
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Builder.Internal
open TimedHostedService
open LifeTimeEventsHostedService

[<EntryPoint>]
let main argv =
    
    let hostConfig (hCfg:IConfigurationBuilder) =
        hCfg.SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("hostsettings", optional = true)
            .AddEnvironmentVariables(prefix = "PREFIX_")
            .AddCommandLine(argv)
            |> ignore
    
    let appConfig (host: HostBuilderContext) (aCfg: IConfigurationBuilder) =
        aCfg.SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional = true)
            .AddJsonFile(
            sprintf "appsettings.%s.json" (host.HostingEnvironment.EnvironmentName)
            , optional = true)
            .AddEnvironmentVariables(prefix =  "PREFIX_")
            .AddCommandLine(argv)
            |> ignore
    
    let serviceConfig (host: HostBuilderContext) (sCfg: IServiceCollection) =
        sCfg.AddLogging()
            .AddHostedService<LifetimeEventsHostedServices>()
            .AddHostedService<TimedHostedService>()
            |> ignore
    
    let loggingConfig (host: HostBuilderContext) (lCfg: ILoggingBuilder) =
        lCfg.AddConsole()
            .AddDebug() 
            |> ignore

    let host = 
        HostBuilder()
            .ConfigureHostConfiguration(Action<IConfigurationBuilder> hostConfig)
            .ConfigureAppConfiguration(Action<HostBuilderContext,IConfigurationBuilder> appConfig)
            .ConfigureServices(Action<HostBuilderContext,IServiceCollection> serviceConfig)
            .ConfigureLogging(Action<HostBuilderContext,ILoggingBuilder> loggingConfig)
            .Build()

    host.RunAsync() |> Async.AwaitTask |> Async.RunSynchronously



    0 // return an integer exit code
