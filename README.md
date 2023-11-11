# How I understood **ConfigureAwait**

1. `ConfigureAwait` works in places where there is a synchronization context.
2. `await` breaks the code into two parts using a state machine and the second part (continuation) is posted to the `Post` method of the synchronization context.
3. Having your own synchronization context, you can control where to invoke the continuation.
4. This example puts the continuations to a queue and has a worker that dequeues continuations and invokes them.
5. The worker runs on the main thread.
6. Having `ConfigureAwait(true)` tells the program to use the same synchronization context for the continuation.
7. If set to `ConfigureAwait(false)` the synchronization context will not be used.