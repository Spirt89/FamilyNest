using System.Text;
using System.Threading.Tasks;
using DataAccess.Models;
using FamilyNest.Services;
using static FamilyNest.Services.SupabaseService;

namespace BusinessLogic.Services
{
    public class ProfilesService : FamilyNest.Services.SupabaseBaseService
    {
        private const string Table = "profiles";

        public ProfilesService(HttpClient httpClient) : base(httpClient) { }

        public async Task<List<ProfileRow>> GetAllAsync()
        {
            var url = $"/rest/v1/{Table}?select=id,full_name,email,avatar_url,updated_at&order=id.asc";
            return await GetListAsync<ProfileRow>(url) ?? new();
        }

        public async Task<ProfileRow?> GetByIdAsync(Guid id)
        {
            var url = $"/rest/v1/{Table}?id=eq.{id}&select=id,full_name,email,avatar_url,updated_at";
            var list = await GetListAsync<ProfileRow>(url);
            return list?.FirstOrDefault();
        }

        public async Task<bool> AddAsync(Guid id, string fullName, string email, string? avatarUrl = null)
        {
            var payload = new { id, full_name = fullName, email, avatar_url = avatarUrl };
            var res = await PostAndReturnAsync<ProfileRow>(Table, payload);
            return res is { Count: > 0 };
        }

        public async Task<bool> UpdateAsync(Guid id, string fullName, string? avatarUrl = null)
        {
            var payload = new { full_name = fullName, avatar_url = avatarUrl };
            var res = await PatchAndReturnAsync<ProfileRow>(Table, $"id=eq.{id}", payload);
            return res is { Count: > 0 };
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var res = await DeleteAndReturnAsync<ProfileRow>(Table, $"id=eq.{id}");
            return res is { Count: > 0 };
        }
    }
}
