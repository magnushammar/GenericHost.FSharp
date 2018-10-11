module TimedHostedService

open System
open System.Threading
open System.Threading.Tasks
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.Logging

type TimedHostedService(logger : ILogger<TimedHostedService>) =
    
    let _logger = logger
    let mutable _timer:Timer = null

    let doWork (state:obj) = _logger.LogInformation(" Timed Backgounr Service is Working")

    interface IHostedService with
        member this.StartAsync( cancellationtoken:CancellationToken) =
            _logger.LogInformation("Timed Background Service is starting.")
            _timer <- new Timer(doWork,null,TimeSpan.Zero, TimeSpan.FromSeconds(5.))
            Task.CompletedTask
        
        member this.StopAsync(cancellationtoken:CancellationToken) =
            _logger.LogInformation("Timed Background Service is stopping")
            _timer.Change(Timeout.Infinite,0) |> ignore
            Task.CompletedTask

    interface IDisposable with
        member this.Dispose() =
            _timer.Dispose()
         