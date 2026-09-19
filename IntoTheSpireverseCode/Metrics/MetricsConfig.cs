namespace IntoTheSpireverse.IntoTheSpireverseCode.Metrics;

internal static class MetricsConfig
{
    public static string? EndpointUrl { get; }

    public static string? IngestKey { get; }

    public static string Environment { get; } =
#if PRODUCTION
        "production";
#else
        "test";
#endif

    static MetricsConfig()
    {
        EndpointUrl = "https://lgjjrqggfddoqumvqlby.supabase.co/rest/v1/run_metrics";
        IngestKey = "sb_publishable_avW2RjBEr28of1JbUyxV4A_vFJdGFTT";
    }
}