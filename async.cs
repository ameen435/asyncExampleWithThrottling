using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

class Program
{
    static async Task Main()
    {
        var urls = new List<string>
        {
            "https://example.com",
            "https://dotnet.microsoft.com",
            "https://learn.microsoft.com",
            "https://github.com"
        };

        // Limit concurrency to 2 tasks at a time
        var semaphore = new SemaphoreSlim(2);
        var tasks = new List<Task>();

        foreach (var url in urls)
        {
            tasks.Add(ProcessUrlAsync(url, semaphore));
        }

        await Task.WhenAll(tasks);
        Console.WriteLine("All requests completed.");
    }

    static async Task ProcessUrlAsync(string url, SemaphoreSlim semaphore)
    {
        await semaphore.WaitAsync();
        try
        {
            using var httpClient = new HttpClient();
            var response = await httpClient.GetStringAsync(url);
            Console.WriteLine($"Fetched {url} - Length: {response.Length}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching {url}: {ex.Message}");
        }
        finally
        {
            semaphore.Release();
        }
    }
}
