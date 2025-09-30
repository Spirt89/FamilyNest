using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.Models;
using FamilyNest.Services;


namespace BusinessLogic.Services
{
        public class FamiliesService : FamilyNest.Services.SupabaseBaseService
        {
            private const string Table = "families";

            public FamiliesService(HttpClient httpClient) : base(httpClient) { }

            public async Task<List<FamilyRow>> GetAllAsync()
            {
                var url = $"/rest/v1/{Table}?select=id,name,created_by,created_at&order=id.asc";
                return await GetListAsync<FamilyRow>(url) ?? new();
            }

            public async Task<FamilyRow?> GetByIdAsync(int id)
            {
                var url = $"/rest/v1/{Table}?id=eq.{id}&select=id,name,created_by,created_at";
                var list = await GetListAsync<FamilyRow>(url);
                return list?.FirstOrDefault();
            }

            public async Task<bool> AddAsync(string name, Guid? createdBy = null)
            {
                var payload = new { name, created_by = createdBy };
                var res = await PostAndReturnAsync<FamilyRow>(Table, payload);
                return res is { Count: > 0 };
            }

            public async Task<bool> UpdateAsync(int id, string name)
            {
                var payload = new { name };
                var res = await PatchAndReturnAsync<FamilyRow>(Table, $"id=eq.{id}", payload);
                return res is { Count: > 0 };
            }

            public async Task<bool> DeleteAsync(int id)
            {
                var res = await DeleteAndReturnAsync<FamilyRow>(Table, $"id=eq.{id}");
                return res is { Count: > 0 };
            }
        }
}
