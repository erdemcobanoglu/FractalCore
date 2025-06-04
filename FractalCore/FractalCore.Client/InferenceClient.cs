using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace FractalCore.Client
{
    public class InferenceClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _orchestratorBaseUrl = "https://localhost:5001"; // API adresini buna göre değiştir

        public InferenceClient()
        {
            _httpClient = new HttpClient();
        }

        public async Task RunAsync()
        {
            Console.WriteLine("Client started...");

            // 1. Segment ID iste (örnek: segment 3)
            int segmentId = 3;
            var segmentBytes = await GetSegmentFromServer(segmentId);

            if (segmentBytes == null)
            {
                Console.WriteLine("Failed to get image segment.");
                return;
            }

            Console.WriteLine($"Segment {segmentId} downloaded. Processing...");

            // 2. (Şimdilik) Mock inference sonucu üret
            var result = new
            {
                SegmentId = segmentId,
                Classification = "Road", // örnek çıktı
                Confidence = 0.94
            };

            // 3. Sonucu API'ye gönder
            bool success = await SubmitResultToServer(result);
            Console.WriteLine(success ? "Result submitted ✅" : "Result submission failed ❌");
        }

        private async Task<byte[]> GetSegmentFromServer(int segmentId)
        {
            try
            {
                var url = $"{_orchestratorBaseUrl}/api/segment/{segmentId}";
                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                    return null;

                return await response.Content.ReadAsByteArrayAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching segment: {ex.Message}");
                return null;
            }
        }

        private async Task<bool> SubmitResultToServer(object resultPayload)
        {
            try
            {
                var url = $"{_orchestratorBaseUrl}/api/result";
                var json = System.Text.Json.JsonSerializer.Serialize(resultPayload);
                var content = new StringContent(json);
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                var response = await _httpClient.PostAsync(url, content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error submitting result: {ex.Message}");
                return false;
            }
        }
    }
}
