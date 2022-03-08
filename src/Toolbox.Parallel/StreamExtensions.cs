using System;
using System.IO;
using System.Threading.Tasks;

namespace Toolbox.Parallel;

public static class StreamExtensions
{
    public static async Task CopyToAsync<TStream>(
        this TStream stream,
        params TStream[] others)
    where TStream : Stream
    {
        var buffer = new byte[8 * 1024];

        int bytesRead;
        while((bytesRead = await stream.ReadAsync(buffer)) > 0)
        {
            foreach (var other in others)
            {
                await other.WriteAsync(buffer.AsMemory(0, bytesRead));
            }
        }
    }
}