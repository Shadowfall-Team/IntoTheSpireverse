using System.Net.Http;
using System.Text;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Saves;

namespace IntoTheSpireverse.IntoTheSpireverseCode.Metrics;


internal static class MetricsUploader
{
    private const string IngestKeyHeaderName = "apikey";

    private static readonly TimeSpan RequestTimeout = TimeSpan.FromSeconds(15);

    public static void Handle(SerializableRun run, bool isVictory, ulong localPlayerId)
    {
        try
        {
            if (!MetricsEligibility.IsEligible(run, localPlayerId))
            {
                return;
            }

            var endpoint = MetricsConfig.EndpointUrl?.Trim();
            var ingestKey = MetricsConfig.IngestKey?.Trim();

            if (!TryValidateConfig(endpoint, ingestKey, out var endpointUri))
            {
                MainFile.Logger.Warn("Skipping run metrics upload: missing or invalid endpoint/ingest key configuration.");
                return;
            }

            var metrics = MetricsPayloadBuilder.BuildRunMetrics(run, isVictory, localPlayerId);
            var payloadJson = MetricsPayloadBuilder.BuildPayloadJson(metrics);

            _ = SendAsync(endpointUri, ingestKey!, payloadJson);
        }
        catch (Exception ex)
        {
            MainFile.Logger.Error("Failed to build run metrics payload: " + ex.Message);
        }
    }


    private static bool TryValidateConfig(string? endpoint, string? ingestKey, out Uri endpointUri)
    {
        endpointUri = null!;

        if (string.IsNullOrEmpty(endpoint) || string.IsNullOrEmpty(ingestKey))
        {
            return false;
        }

        if (!Uri.TryCreate(endpoint, UriKind.Absolute, out var uri) || uri.Scheme != Uri.UriSchemeHttps)
        {
            return false;
        }

        endpointUri = uri;
        return true;
    }


    private static async Task SendAsync(Uri endpoint, string ingestKey, string json)
    {
        try
        {
            // MainFile.Logger.Info("Uploading run metrics...");

            using var handler = new HttpClientHandler { AllowAutoRedirect = true };
            using var client = new HttpClient(handler) { Timeout = RequestTimeout };
            using var content = new StringContent(json, Encoding.UTF8, "application/json");
            using var request = new HttpRequestMessage(HttpMethod.Post, endpoint) { Content = content };

            request.Headers.Add(IngestKeyHeaderName, ingestKey);
            request.Headers.Add("Prefer", "return=minimal");

            using var response = await client
                .SendAsync(request, HttpCompletionOption.ResponseHeadersRead)
                .ConfigureAwait(false);

            // if (response.IsSuccessStatusCode)
            // {
            //     MainFile.Logger.Info("Run metrics upload succeeded.");
            // }
            // else
            // {
            //     MainFile.Logger.Warn(
            //         $"Run metrics upload failed with status {(int)response.StatusCode} {response.StatusCode}.");
            // }
        }
        catch (Exception ex)
        {
            MainFile.Logger.Error("Run metrics upload failed: " + ex.Message);
        }
    }
}
