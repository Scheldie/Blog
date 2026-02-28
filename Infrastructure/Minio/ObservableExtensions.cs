using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Minio.DataModel;

namespace Blog.Infrastructure.Minio;

public static class ObservableExtensions
{
    public static async IAsyncEnumerable<Item> ToAsyncEnumerable(
        this IObservable<Item> source,
        [EnumeratorCancellation] CancellationToken ct = default)
    {
        var queue = new Queue<Item>();
        var tcs = new TaskCompletionSource();

        using var subscription = source.Subscribe(
            item => queue.Enqueue(item),
            ex => tcs.TrySetException(ex),
            () => tcs.TrySetResult());

        while (true)
        {
            while (queue.Count > 0)
                yield return queue.Dequeue();

            if (tcs.Task.IsCompleted)
                break;

            await Task.WhenAny(tcs.Task, Task.Delay(10, ct));
        }

        if (tcs.Task.IsFaulted)
            throw tcs.Task.Exception;
    }
}
