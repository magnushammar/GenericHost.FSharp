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
    
    let host = 
        HostBuilder()
            .ConfigureHostConfiguration(fun (hCfg:IConfigurationBuilder) ->
                hCfg.SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("hostsettings", optional = true)
                    .AddEnvironmentVariables(prefix = "PREFIX_")
                    .AddCommandLine(argv)
                    |> ignore)
            .ConfigureAppConfiguration(fun (host: HostBuilderContext) (aCfg: IConfigurationBuilder) ->
                aCfg.SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional = true)
                    .AddJsonFile(
                    sprintf "appsettings.%s.json" (host.HostingEnvironment.EnvironmentName)
                    , optional = true)
                    .AddEnvironmentVariables(prefix =  "PREFIX_")
                    .AddCommandLine(argv)
                    |> ignore)
            .ConfigureServices(fun (host: HostBuilderContext) (sCfg: IServiceCollection) -> 
                sCfg.AddLogging()
                    .AddHostedService<LifetimeEventsHostedServices>()
                    .AddHostedService<TimedHostedService>()
                    |> ignore)
            .ConfigureLogging(fun (host: HostBuilderContext) (lCfg: ILoggingBuilder) ->
                lCfg.AddConsole()
                    .AddDebug() 
                    |> ignore)
            .Build()

    host.RunAsync() |> Async.AwaitTask |> Async.RunSynchronously



    0 // return an integer exit code
