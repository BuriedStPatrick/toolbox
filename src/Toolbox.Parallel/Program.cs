using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Toolbox.Parallel;

public static class Program
{
    public static async Task Main(string[] args)
    {
        await using var inputStream = Console.OpenStandardInput();
        using var inputReader = new StreamReader(
            inputStream,
            Console.InputEncoding);

        var input = await inputReader.ReadToEndAsync();
        var (threads, outFile, key, command) = GetOptions(args);
        var outputStream = Console.OpenStandardOutput();
        var semaphore = new SemaphoreSlim(threads);

        var tasks = input.Split(Environment.NewLine)
            .Where(line => !string.IsNullOrEmpty(line))
            .Select(async line =>
            {
                await semaphore.WaitAsync();

                var renderedCommand = command.Replace(
                    key,
                    line);

                await Run(
                    renderedCommand.Split(" "),
                    outputStream,
                    outFile?.Replace(key, line) ?? null);

                semaphore.Release();
            }).ToList();

        await Task.WhenAll(tasks);
    }

    private static string? TryGetValue(string[] args, string key)
    {
        var keyIndex = Array.IndexOf(args, key);
        return keyIndex < 0 ? null : args[keyIndex + 1];
    }

    private static int? TryGetIntValue(string[] args, string key)
    {
        return int.TryParse(TryGetValue(args, key), out var result)
            ? result
            : null;
    }

    private static async Task Run(
        string[] args,
        Stream stream,
        string? outFile = null)
    {
        using var pProcess = new System.Diagnostics.Process();
        pProcess.StartInfo.FileName = args[0];
        pProcess.StartInfo.Arguments = string.Join(" ", args[1..]);
        pProcess.StartInfo.UseShellExecute = false;
        pProcess.StartInfo.RedirectStandardOutput = true;
        pProcess.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
        pProcess.StartInfo.CreateNoWindow = true;
        pProcess.Start();

        var output = pProcess.StandardOutput.BaseStream;

        if (outFile is not null)
        {
            await using var fileStream = File.OpenWrite(outFile);
            await output.CopyToAsync(
                stream,
                fileStream);
        }
        else
        {
            await output.CopyToAsync(stream);
        }

        await pProcess.WaitForExitAsync();
    }

    private static Options GetOptions(string[] args) =>
        new(
            TryGetIntValue(args, "--threads") ?? 5,
            TryGetValue(args, "--out-file"),
            TryGetValue(args, "--key") ?? "%s",
            string.Join(" ", args[(Array.IndexOf(args, "--")+1)..]));
}

public record Options(
    int Threads,
    string? OutFile,
    string Key,
    string Command);
