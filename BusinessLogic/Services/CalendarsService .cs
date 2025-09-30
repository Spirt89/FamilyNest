using System.Text;
using System.Threading.Tasks;
using DataAccess.Models;
using FamilyNest.Services;
using static FamilyNest.Services.SupabaseService;

namespace BusinessLogic.Services
{
    public class CalendarsService : FamilyNest.Services.SupabaseBaseService
    {
        private const string CalendarsTable = "calendars";
        private const string SharesTable = "calendar_shares";

        public CalendarsService(HttpClient httpClient) : base(httpClient) { }

        // Calendars
        public async Task<List<CalendarRow>> GetByFamilyIdAsync(int familyId)
        {
            var url = $"/rest/v1/{CalendarsTable}?family_id=eq.{familyId}&select=id,family_id,name,created_by,created_at&order=id.asc";
            return await GetListAsync<CalendarRow>(url) ?? new();
        }

        public async Task<CalendarRow?> GetByIdAsync(int id)
        {
            var url = $"/rest/v1/{CalendarsTable}?id=eq.{id}&select=id,family_id,name,created_by,created_at";
            var list = await GetListAsync<CalendarRow>(url);
            return list?.FirstOrDefault();
        }

        public async Task<bool> AddAsync(int familyId, string name, Guid? createdBy = null)
        {
            var payload = new { family_id = familyId, name, created_by = createdBy };
            var res = await PostAndReturnAsync<CalendarRow>(CalendarsTable, payload);
            return res is { Count: > 0 };
        }

        public async Task<bool> UpdateAsync(int id, string name)
        {
            var payload = new { name };
            var res = await PatchAndReturnAsync<CalendarRow>(CalendarsTable, $"id=eq.{id}", payload);
            return res is { Count: > 0 };
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var res = await DeleteAndReturnAsync<CalendarRow>(CalendarsTable, $"id=eq.{id}");
            return res is { Count: > 0 };
        }

        // Calendar shares
        public async Task<List<CalendarShareRow>> GetSharesByCalendarIdAsync(int calendarId)
        {
            var url = $"/rest/v1/{SharesTable}?calendar_id=eq.{calendarId}&select=id,calendar_id,user_id,permission,created_at&order=id.asc";
            return await GetListAsync<CalendarShareRow>(url) ?? new();
        }

        public async Task<bool> AddShareAsync(int calendarId, Guid userId, string permission)
        {
            var payload = new { calendar_id = calendarId, user_id = userId, permission };
            var res = await PostAndReturnAsync<CalendarShareRow>(SharesTable, payload);
            return res is { Count: > 0 };
        }

        public async Task<bool> DeleteShareAsync(int id)
        {
            var res = await DeleteAndReturnAsync<CalendarShareRow>(SharesTable, $"id=eq.{id}");
            return res is { Count: > 0 };
        }
    }
}
