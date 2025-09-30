using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace FamilyNest.Services
{
    public abstract class SupabaseBaseService
    {
        protected readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _json = new() { PropertyNameCaseInsensitive = true };

        protected SupabaseBaseService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        protected async Task<List<T>?> GetListAsync<T>(string url)
        {
            var resp = await _httpClient.GetAsync(url);
            resp.EnsureSuccessStatusCode();
            var json = await resp.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<T>>(json, _json);
        }

        protected async Task<List<T>?> PostAndReturnAsync<T>(string table, object payload)
        {
            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            var req = new HttpRequestMessage(HttpMethod.Post, $"/rest/v1/{table}") { Content = content };
            req.Headers.Add("Prefer", "return=representation");
            var resp = await _httpClient.SendAsync(req);
            if (!resp.IsSuccessStatusCode) return null;
            var body = await resp.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<T>>(body, _json);
        }

        protected async Task<List<T>?> PatchAndReturnAsync<T>(string table, string whereClause, object payload)
        {
            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            var req = new HttpRequestMessage(new HttpMethod("PATCH"), $"/rest/v1/{table}?{whereClause}") { Content = content };
            req.Headers.Add("Prefer", "return=representation");
            var resp = await _httpClient.SendAsync(req);
            if (!resp.IsSuccessStatusCode) return null;
            var body = await resp.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<T>>(body, _json);
        }

        protected async Task<List<T>?> DeleteAndReturnAsync<T>(string table, string whereClause)
        {
            var req = new HttpRequestMessage(HttpMethod.Delete, $"/rest/v1/{table}?{whereClause}");
            req.Headers.Add("Prefer", "return=representation");
            var resp = await _httpClient.SendAsync(req);
            if (!resp.IsSuccessStatusCode) return null;
            var body = await resp.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<T>>(body, _json);
        }
    }
}
