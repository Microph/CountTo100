using System;
using System.Threading;
using System.Threading.Tasks;

namespace CountTo100.Utilities
{
    public static class TaskHelper
    {
        public static async Task When(Func<bool> predicate, CancellationToken cancellationToken)
        {
            while (!predicate.Invoke())
            {
                cancellationToken.ThrowIfCancellationRequested();
                await Task.Yield();
            }
        }

        public static Task When(Func<bool> predicate)
        {
            return When(predicate, CancellationToken.None);
        }
    }
}