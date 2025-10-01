using System.Threading.Channels;
using ProductManagement.Services.Abstract;

namespace ProductManagement.Services.Background;

public class BackgroundTaskQueue : IBackgroundTaskQueue
{
  private readonly Channel<Guid> _queue = Channel.CreateUnbounded<Guid>();

  public async ValueTask QueueBackgroundWorkItemAsync(Guid imageId)
  {
    await _queue.Writer.WriteAsync(imageId);
  }

  public async ValueTask<Guid?> DequeueAsync(CancellationToken cancellationToken)
  {
    try
    {
      var id = await _queue.Reader.ReadAsync(cancellationToken);
      return id;
    }
    catch (OperationCanceledException )
    {
      return null;
    }
  }

}