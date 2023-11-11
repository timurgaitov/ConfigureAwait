using System.Collections.Concurrent;

namespace ConfigureAwait;

public static class Program
{
    public static void Main()
    {
        var syncContext = new SyncContext();

        _ = Task.Run(async () =>
        {
            SynchronizationContext.SetSynchronizationContext(syncContext);
        
            Console.WriteLine($"{Environment.CurrentManagedThreadId}: BeforeAwait"); // Y: BeforeAwait
            
            await Task.Delay(1000).ConfigureAwait(true);
            
            Console.WriteLine($"{Environment.CurrentManagedThreadId}: AfterAwait"); // X: AfterAwait
        });

        syncContext.StartSyncContextWorker();
    }

    private class SyncContext : SynchronizationContext
    {
        private readonly ConcurrentQueue<Action> _queue = new();
        
        public override void Post(SendOrPostCallback d, object? state)
        {
            Console.WriteLine($"{Environment.CurrentManagedThreadId}: Post"); // Y: Post
            
            _queue.Enqueue(() => d.Invoke(state));
        }
    
        public void StartSyncContextWorker()
        {
            Console.WriteLine($"{Environment.CurrentManagedThreadId}: StartSyncContextWorker"); // X: StartSyncContextWorker
            
            while (true)
            {
                if (_queue.TryDequeue(out var afterAwait))
                {
                    afterAwait();
                }
            }
        }
    }
}
