using DataAccess.Models;
using FamilyNest.Services;
using static FamilyNest.Services.SupabaseService;

namespace BusinessLogic.Services
{
    public class EventsService : FamilyNest.Services.SupabaseBaseService
    {
        private const string Table = "events";

        public EventsService(HttpClient httpClient) : base(httpClient) { }

        public async Task<List<EventRow>> GetByCalendarIdAsync(int calendarId)
        {
            var url = $"/rest/v1/{Table}?calendar_id=eq.{calendarId}&select=id,calendar_id,title,description,start_at,end_at,created_by&order=start_at.asc";
            return await GetListAsync<EventRow>(url) ?? new();
        }

        public async Task<EventRow?> GetByIdAsync(int id)
        {
            var url = $"/rest/v1/{Table}?id=eq.{id}&select=id,calendar_id,title,description,start_at,end_at,created_by";
            var list = await GetListAsync<EventRow>(url);
            return list?.FirstOrDefault();
        }

        public async Task<bool> AddAsync(int calendarId, string title, string? description, DateTime startAt, DateTime? endAt, Guid? createdBy = null)
        {
            var payload = new { calendar_id = calendarId, title, description, start_at = startAt, end_at = endAt, created_by = createdBy };
            var res = await PostAndReturnAsync<EventRow>(Table, payload);
            return res is { Count: > 0 };
        }

        public async Task<bool> UpdateAsync(int id, string title, string? description, DateTime? startAt = null, DateTime? endAt = null)
        {
            var payload = new { title, description, start_at = startAt, end_at = endAt };
            var res = await PatchAndReturnAsync<EventRow>(Table, $"id=eq.{id}", payload);
            return res is { Count: > 0 };
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var res = await DeleteAndReturnAsync<EventRow>(Table, $"id=eq.{id}");
            return res is { Count: > 0 };
        }
    }
}
