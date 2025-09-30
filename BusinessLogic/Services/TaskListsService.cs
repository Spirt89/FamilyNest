using DataAccess.Models;
using FamilyNest.Services;
using static FamilyNest.Services.SupabaseService;

namespace BusinessLogic.Services
{
    public class TaskListsService : FamilyNest.Services.SupabaseBaseService
    {
        private const string ListsTable = "task_lists";
        private const string TasksTable = "tasks";
        private const string AssignmentsTable = "task_assignments";

        public TaskListsService(HttpClient httpClient) : base(httpClient) { }

        // ----- Task Lists -----
        public async Task<List<TaskListRow>> GetByFamilyIdAsync(int familyId)
        {
            var url = $"/rest/v1/{ListsTable}?family_id=eq.{familyId}&select=id,family_id,name,created_at&order=id.asc";
            return await GetListAsync<TaskListRow>(url) ?? new();
        }

        public async Task<TaskListRow?> GetByIdAsync(int id)
        {
            var url = $"/rest/v1/{ListsTable}?id=eq.{id}&select=id,family_id,name,created_at";
            var list = await GetListAsync<TaskListRow>(url);
            return list?.FirstOrDefault();
        }

        public async Task<bool> AddAsync(int familyId, string name)
        {
            var payload = new { family_id = familyId, name };
            var res = await PostAndReturnAsync<TaskListRow>(ListsTable, payload);
            return res is { Count: > 0 };
        }

        public async Task<bool> UpdateAsync(int id, string name)
        {
            var payload = new { name };
            var res = await PatchAndReturnAsync<TaskListRow>(ListsTable, $"id=eq.{id}", payload);
            return res is { Count: > 0 };
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var res = await DeleteAndReturnAsync<TaskListRow>(ListsTable, $"id=eq.{id}");
            return res is { Count: > 0 };
        }

        // ----- Tasks -----
        public async Task<List<TaskRow>> GetTasksByListIdAsync(int listId)
        {
            var url = $"/rest/v1/{TasksTable}?task_list_id=eq.{listId}&select=id,task_list_id,title,details,due_at,priority,repeats,created_by,completed,completed_at&order=id.asc";
            return await GetListAsync<TaskRow>(url) ?? new();
        }

        public async Task<TaskRow?> GetTaskByIdAsync(int id)
        {
            var url = $"/rest/v1/{TasksTable}?id=eq.{id}&select=id,task_list_id,title,details,due_at,priority,repeats,created_by,completed,completed_at";
            var list = await GetListAsync<TaskRow>(url);
            return list?.FirstOrDefault();
        }

        public async Task<bool> AddTaskAsync(int listId, string title, string? details = null, DateTime? dueAt = null, int? priority = null, string? repeats = null, Guid? createdBy = null)
        {
            var payload = new { task_list_id = listId, title, details, due_at = dueAt, priority, repeats, created_by = createdBy };
            var res = await PostAndReturnAsync<TaskRow>(TasksTable, payload);
            return res is { Count: > 0 };
        }

        public async Task<bool> UpdateTaskAsync(int id, string title, string? details = null, DateTime? dueAt = null, int? priority = null, string? repeats = null, bool? completed = null)
        {
            var payload = new { title, details, due_at = dueAt, priority, repeats, completed, completed_at = (completed == true ? DateTime.UtcNow : (DateTime?)null) };
            var res = await PatchAndReturnAsync<TaskRow>(TasksTable, $"id=eq.{id}", payload);
            return res is { Count: > 0 };
        }

        public async Task<bool> DeleteTaskAsync(int id)
        {
            var res = await DeleteAndReturnAsync<TaskRow>(TasksTable, $"id=eq.{id}");
            return res is { Count: > 0 };
        }

        // ----- Task Assignments -----
        public async Task<List<TaskAssignmentRow>> GetAssignmentsByTaskIdAsync(int taskId)
        {
            var url = $"/rest/v1/{AssignmentsTable}?task_id=eq.{taskId}&select=id,task_id,user_id,assigned_by,assigned_at&order=id.asc";
            return await GetListAsync<TaskAssignmentRow>(url) ?? new();
        }

        public async Task<bool> AddAssignmentAsync(int taskId, Guid userId, Guid? assignedBy = null)
        {
            var payload = new { task_id = taskId, user_id = userId, assigned_by = assignedBy };
            var res = await PostAndReturnAsync<TaskAssignmentRow>(AssignmentsTable, payload);
            return res is { Count: > 0 };
        }

        public async Task<bool> DeleteAssignmentAsync(int id)
        {
            var res = await DeleteAndReturnAsync<TaskAssignmentRow>(AssignmentsTable, $"id=eq.{id}");
            return res is { Count: > 0 };
        }
    }
}
