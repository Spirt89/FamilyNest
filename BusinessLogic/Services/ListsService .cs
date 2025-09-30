using DataAccess.Models;
using FamilyNest.Services;
using static FamilyNest.Services.SupabaseService;

namespace BusinessLogic.Services
{
    public class ListsService : FamilyNest.Services.SupabaseBaseService
    {
        private const string ListsTable = "lists";
        private const string ItemsTable = "list_items";

        public ListsService(HttpClient httpClient) : base(httpClient) { }

        // ----- Lists -----
        public async Task<List<ListRow>> GetByFamilyIdAsync(int familyId)
        {
            var url = $"/rest/v1/{ListsTable}?family_id=eq.{familyId}&select=id,family_id,name,created_at&order=id.asc";
            return await GetListAsync<ListRow>(url) ?? new();
        }

        public async Task<ListRow?> GetByIdAsync(int id)
        {
            var url = $"/rest/v1/{ListsTable}?id=eq.{id}&select=id,family_id,name,created_at";
            var list = await GetListAsync<ListRow>(url);
            return list?.FirstOrDefault();
        }

        public async Task<bool> AddAsync(int familyId, string name)
        {
            var payload = new { family_id = familyId, name };
            var res = await PostAndReturnAsync<ListRow>(ListsTable, payload);
            return res is { Count: > 0 };
        }

        public async Task<bool> UpdateAsync(int id, string name)
        {
            var payload = new { name };
            var res = await PatchAndReturnAsync<ListRow>(ListsTable, $"id=eq.{id}", payload);
            return res is { Count: > 0 };
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var res = await DeleteAndReturnAsync<ListRow>(ListsTable, $"id=eq.{id}");
            return res is { Count: > 0 };
        }

        // ----- List Items -----
        public async Task<List<ListItemRow>> GetItemsByListIdAsync(int listId)
        {
            var url = $"/rest/v1/{ItemsTable}?list_id=eq.{listId}&select=id,list_id,content,quantity,checked,created_by,created_at&order=id.asc";
            return await GetListAsync<ListItemRow>(url) ?? new();
        }

        public async Task<bool> AddItemAsync(int listId, string content, string? quantity = null, Guid? createdBy = null)
        {
            var payload = new { list_id = listId, content, quantity, created_by = createdBy };
            var res = await PostAndReturnAsync<ListItemRow>(ItemsTable, payload);
            return res is { Count: > 0 };
        }

        public async Task<bool> UpdateItemAsync(int id, string? content = null, bool? checkedFlag = null)
        {
            var payload = new { content, @checked = checkedFlag };
            var res = await PatchAndReturnAsync<ListItemRow>(ItemsTable, $"id=eq.{id}", payload);
            return res is { Count: > 0 };
        }

        public async Task<bool> DeleteItemAsync(int id)
        {
            var res = await DeleteAndReturnAsync<ListItemRow>(ItemsTable, $"id=eq.{id}");
            return res is { Count: > 0 };
        }
    }
}
