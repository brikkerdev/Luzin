using System.Diagnostics.Metrics;

namespace MusicWeb.Observability;

public static class SongMetrics
{
    public const string MeterName = "MusicWeb";
    private static readonly Meter Meter = new(MeterName, "1.0.0");

    public static readonly Counter<long> Requests =
        Meter.CreateCounter<long>("musicweb_songs_requests_total", description: "Songs service requests");

    public static readonly Histogram<double> DurationMs =
        Meter.CreateHistogram<double>("musicweb_songs_duration_ms", unit: "ms", description: "Songs service duration");

    public static readonly Counter<long> CacheHits =
        Meter.CreateCounter<long>("musicweb_songs_cache_hits_total", description: "Songs cache hits");

    public static readonly Counter<long> CacheMisses =
        Meter.CreateCounter<long>("musicweb_songs_cache_misses_total", description: "Songs cache misses");

    public static readonly Histogram<double> DbDurationMs =
        Meter.CreateHistogram<double>("musicweb_songs_db_duration_ms", unit: "ms", description: "Songs DB query duration");
}