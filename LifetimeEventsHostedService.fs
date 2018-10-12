module LifeTimeEventsHostedService

open System
open System.Threading
open System.Threading.Tasks
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.Logging

type internal LifetimeEventsHostedServices(logger : ILogger<LifetimeEventsHostedServices>, appLifetime : IApplicationLifetime) =
    
    let _logger = logger
    let _appLifetime = appLifetime
    let onStarted() = _logger.LogInformation "OnStarted has been called"
    let onStopping() = _logger.LogInformation "OnStopping has been called"
    let onStopped() = _logger.LogInformation "OnStopped has been called"

    interface IHostedService with
        
        member this.StartAsync( cancellationtoken:CancellationToken) =
            let myaction = Action onStarted
            _appLifetime.ApplicationStarted.Register(Action onStarted ) |> ignore
            _appLifetime.ApplicationStopping.Register(Action onStopping) |> ignore
            _appLifetime.ApplicationStopped.Register(Action onStopped) |> ignore
            Task.CompletedTask
        
        member this.StopAsync(cancellationtoken:CancellationToken) =
            _logger.LogInformation("Timed Background Service is stopping")
            Task.CompletedTask

        

         