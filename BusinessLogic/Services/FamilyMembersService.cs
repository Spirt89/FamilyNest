using System.Text;
using System.Threading.Tasks;
using DataAccess.Models;
using FamilyNest.Services;
using static FamilyNest.Services.SupabaseService;

namespace BusinessLogic.Services
{
    public class FamilyMembersService : FamilyNest.Services.SupabaseBaseService
    {
        private const string Table = "family_members";

        public FamilyMembersService(HttpClient httpClient) : base(httpClient) { }

        public async Task<List<FamilyMemberRow>> GetByFamilyIdAsync(int familyId)
        {
            var url = $"/rest/v1/{Table}?family_id=eq.{familyId}&select=id,family_id,user_id,role,created_by,created_at&order=id.asc";
            return await GetListAsync<FamilyMemberRow>(url) ?? new();
        }

        public async Task<FamilyMemberRow?> GetByIdAsync(int id)
        {
            var url = $"/rest/v1/{Table}?id=eq.{id}&select=id,family_id,user_id,role,created_by,created_at";
            var list = await GetListAsync<FamilyMemberRow>(url);
            return list?.FirstOrDefault();
        }

        public async Task<bool> AddAsync(int familyId, Guid userId, string? role = null, Guid? createdBy = null)
        {
            var payload = new { family_id = familyId, user_id = userId, role, created_by = createdBy };
            var res = await PostAndReturnAsync<FamilyMemberRow>(Table, payload);
            return res is { Count: > 0 };
        }

        public async Task<bool> UpdateAsync(int id, string? role = null)
        {
            var payload = new { role };
            var res = await PatchAndReturnAsync<FamilyMemberRow>(Table, $"id=eq.{id}", payload);
            return res is { Count: > 0 };
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var res = await DeleteAndReturnAsync<FamilyMemberRow>(Table, $"id=eq.{id}");
            return res is { Count: > 0 };
        }
    }
}
