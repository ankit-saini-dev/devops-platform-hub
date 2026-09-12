using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace DevOpsPlatformHub.IntegrationTests;

public sealed class TestLogProvider : ILoggerProvider
{
    private readonly ConcurrentQueue<CapturedLogEntry> _entries = new();
    public IReadOnlyCollection<CapturedLogEntry> Entries => [.. _entries];

    public ILogger CreateLogger(string categoryName)
    {
        return new TestLogger(categoryName, _entries);
    }

    public void Dispose()
    {
    }

    public void Clear()
    {
        while (_entries.TryDequeue(out _))
        {
        }
    }

    private sealed class TestLogger(string categoryName, ConcurrentQueue<CapturedLogEntry> entries) : ILogger
    {
        public IDisposable? BeginScope<TState>(TState state) where TState : notnull
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception,
            Func<TState, Exception?, string> formatter)
        {
            var properties = state as IReadOnlyList<KeyValuePair<string, object?>>
                             ?? [];
            entries.Enqueue(new CapturedLogEntry(categoryName, logLevel, properties, exception));
        }
    }
}

public sealed record CapturedLogEntry(
    string CategoryName,
    LogLevel LogLevel,
    IReadOnlyList<KeyValuePair<string, object?>> Properties,
    Exception? Exception);
