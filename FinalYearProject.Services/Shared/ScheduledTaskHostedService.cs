using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace FinalYearProject.Services.Shared;

public class ScheduledTaskHostedService(ILogger<ScheduledTaskHostedService> logger) : IHostedService, IDisposable
{
    private readonly ConcurrentDictionary<Guid, Timer> _timers = new();

    public Task StartAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Delayed Task Hosted Service started.");
        return Task.CompletedTask;
    }

    /// <summary>
    /// Run a task after a specified time and returns the task Id
    /// </summary>
    /// <param name="task"></param>
    /// <param name="runAfter"></param>
    /// <returns>Returns <see cref="Guid"/> </returns>
    public async Task<Guid> ScheduleTask(Action task, DateTime? runAfter = null)
    {
        logger.LogInformation("Executing scheduled task... Scheduled at {time}", runAfter);
        DateTime runAt = runAfter ?? DateTime.UtcNow;
        TimeSpan delay = runAt - DateTime.UtcNow;
        if (delay <= TimeSpan.Zero)
        {
            task();
            return Guid.Empty;
        }

        var timerId = Guid.NewGuid();
        var timer = new Timer(_ =>
        {
            task();
            _timers.TryRemove(timerId, out Timer? _);
        }, null, delay, Timeout.InfiniteTimeSpan);

        _timers.TryAdd(timerId, timer);
        return await Task.FromResult(timerId);
    }

    /// <summary>
    /// Cancel task with the specified ID
    /// </summary>
    /// <param name="taskId"></param>
    public void CancelTask(Guid taskId)
    {

        if (_timers.TryRemove(taskId, out var timer))
        {
            timer.Dispose();
            logger.LogInformation("Scheduled task with ID {id} canceled", taskId);
        }
        else
        {
            logger.LogInformation("Scheduled task with ID {id} was not found while trying to cancel task", taskId);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        foreach (var timer in _timers.Values) timer.Dispose();
        _timers.Clear();
        logger.LogInformation("Delayed Task Hosted Service stopped.");
        return Task.CompletedTask;
    }

    public void Dispose() => StopAsync(CancellationToken.None).Wait();
}
