namespace ProductManagement.Services.Abstract;

public interface IBackgroundTaskQueue
{
  ValueTask QueueBackgroundWorkItemAsync(Guid imageId);
  ValueTask<Guid?> DequeueAsync(CancellationToken cancellationToken);
}