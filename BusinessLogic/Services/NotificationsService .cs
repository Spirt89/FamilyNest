using System.Text.Json;
using DataAccess.Models;
using FamilyNest.Services;
using static FamilyNest.Services.SupabaseService;

namespace BusinessLogic.Services
{
    public class NotificationsService : FamilyNest.Services.SupabaseBaseService
    {
        private const string Table = "notifications";

        public NotificationsService(HttpClient httpClient) : base(httpClient) { }

        public async Task<List<NotificationRow>> GetByUserIdAsync(Guid userId)
        {
            var url = $"/rest/v1/{Table}?user_id=eq.{userId}&select=id,user_id,type,payload,read,created_at&order=id.desc";
            return await GetListAsync<NotificationRow>(url) ?? new();
        }

        public async Task<bool> AddAsync(Guid userId, string type, object? payload = null)
        {
            var payloadObj = new { user_id = userId, type, payload = payload == null ? null : JsonSerializer.Serialize(payload), read = false };
            var res = await PostAndReturnAsync<NotificationRow>(Table, payloadObj);
            return res is { Count: > 0 };
        }

        public async Task<bool> MarkAsReadAsync(int id)
        {
            var payload = new { read = true };
            var res = await PatchAndReturnAsync<NotificationRow>(Table, $"id=eq.{id}", payload);
            return res is { Count: > 0 };
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var res = await DeleteAndReturnAsync<NotificationRow>(Table, $"id=eq.{id}");
            return res is { Count: > 0 };
        }
    }
}
