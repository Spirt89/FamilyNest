using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace FamilyNest.Services
{
    public class SupabaseService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _json = new() { PropertyNameCaseInsensitive = true };

        public SupabaseService(string supabaseUrl, string apiKey)
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(supabaseUrl.TrimEnd('/'))
            };

            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", apiKey);
            _httpClient.DefaultRequestHeaders.Add("apikey", apiKey);
            _httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        }

        #region Helpers

        private async Task<List<T>?> GetListAsync<T>(string url)
        {
            var resp = await _httpClient.GetAsync(url);
            resp.EnsureSuccessStatusCode();
            var json = await resp.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<T>>(json, _json);
        }

        private async Task<List<T>?> PostAndReturnAsync<T>(string table, object payload)
        {
            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            var req = new HttpRequestMessage(HttpMethod.Post, $"/rest/v1/{table}")
            {
                Content = content
            };
            req.Headers.Add("Prefer", "return=representation");
            var resp = await _httpClient.SendAsync(req);
            if (!resp.IsSuccessStatusCode) return null;
            var body = await resp.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<T>>(body, _json);
        }

        private async Task<List<T>?> PatchAndReturnAsync<T>(string table, string whereClause, object payload)
        {
            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            var req = new HttpRequestMessage(new HttpMethod("PATCH"), $"/rest/v1/{table}?{whereClause}")
            {
                Content = content
            };
            req.Headers.Add("Prefer", "return=representation");
            var resp = await _httpClient.SendAsync(req);
            if (!resp.IsSuccessStatusCode) return null;
            var body = await resp.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<T>>(body, _json);
        }

        private async Task<List<T>?> DeleteAndReturnAsync<T>(string table, string whereClause)
        {
            var req = new HttpRequestMessage(HttpMethod.Delete, $"/rest/v1/{table}?{whereClause}");
            req.Headers.Add("Prefer", "return=representation");
            var resp = await _httpClient.SendAsync(req);
            if (!resp.IsSuccessStatusCode) return null;
            var body = await resp.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<T>>(body, _json);
        }

        #endregion

        #region families (пример — сохраняю для совместимости)
        private const string FamiliesTable = "families";

        public async Task<List<FamilyRow>> GetAllFamiliesAsync()
        {
            var url = $"/rest/v1/{FamiliesTable}?select=id,name,created_by,created_at&order=id.asc";
            return await GetListAsync<FamilyRow>(url) ?? new();
        }

        public async Task<FamilyRow?> GetFamilyByIdAsync(int id)
        {
            var url = $"/rest/v1/{FamiliesTable}?id=eq.{id}&select=id,name,created_by,created_at";
            var list = await GetListAsync<FamilyRow>(url);
            return list?.FirstOrDefault();
        }

        public async Task<bool> AddFamilyAsync(string name, Guid? createdBy = null)
        {
            var payload = new { name, created_by = createdBy };
            var res = await PostAndReturnAsync<FamilyRow>(FamiliesTable, payload);
            return res is { Count: > 0 };
        }

        public async Task<bool> UpdateFamilyAsync(int id, string name)
        {
            var payload = new { name };
            var res = await PatchAndReturnAsync<FamilyRow>(FamiliesTable, $"id=eq.{id}", payload);
            return res is { Count: > 0 };
        }

        public async Task<bool> DeleteFamilyAsync(int id)
        {
            var res = await DeleteAndReturnAsync<FamilyRow>(FamiliesTable, $"id=eq.{id}");
            return res is { Count: > 0 };
        }

        public class FamilyRow
        {
            public int Id { get; set; }
            public string Name { get; set; } = string.Empty;
            public Guid? Created_By { get; set; }
            public DateTime? Created_At { get; set; }
        }
        #endregion

        #region profiles
        private const string ProfilesTable = "profiles";

        public async Task<List<ProfileRow>> GetAllProfilesAsync()
        {
            var url = $"/rest/v1/{ProfilesTable}?select=id,full_name,email,avatar_url,updated_at&order=id.asc";
            return await GetListAsync<ProfileRow>(url) ?? new();
        }

        public async Task<ProfileRow?> GetProfileByIdAsync(Guid id)
        {
            var url = $"/rest/v1/{ProfilesTable}?id=eq.{id}&select=id,full_name,email,avatar_url,updated_at";
            var list = await GetListAsync<ProfileRow>(url);
            return list?.FirstOrDefault();
        }

        public async Task<bool> AddProfileAsync(Guid id, string fullName, string email, string? avatarUrl = null)
        {
            var payload = new { id, full_name = fullName, email, avatar_url = avatarUrl };
            var res = await PostAndReturnAsync<ProfileRow>(ProfilesTable, payload);
            return res is { Count: > 0 };
        }

        public async Task<bool> UpdateProfileAsync(Guid id, string fullName, string? avatarUrl = null)
        {
            var payload = new { full_name = fullName, avatar_url = avatarUrl };
            var res = await PatchAndReturnAsync<ProfileRow>(ProfilesTable, $"id=eq.{id}", payload);
            return res is { Count: > 0 };
        }

        public async Task<bool> DeleteProfileAsync(Guid id)
        {
            var res = await DeleteAndReturnAsync<ProfileRow>(ProfilesTable, $"id=eq.{id}");
            return res is { Count: > 0 };
        }

        public class ProfileRow
        {
            public Guid Id { get; set; }
            public string? Full_Name { get; set; }
            public string? Email { get; set; }
            public string? Avatar_Url { get; set; }
            public DateTime? Updated_At { get; set; }
        }
        #endregion

        #region family_members
        private const string FamilyMembersTable = "family_members";

        public async Task<List<FamilyMemberRow>> GetFamilyMembersByFamilyIdAsync(int familyId)
        {
            var url = $"/rest/v1/{FamilyMembersTable}?family_id=eq.{familyId}&select=id,family_id,user_id,role,created_by,created_at&order=id.asc";
            return await GetListAsync<FamilyMemberRow>(url) ?? new();
        }

        public async Task<FamilyMemberRow?> GetFamilyMemberByIdAsync(int id)
        {
            var url = $"/rest/v1/{FamilyMembersTable}?id=eq.{id}&select=id,family_id,user_id,role,created_by,created_at";
            var list = await GetListAsync<FamilyMemberRow>(url);
            return list?.FirstOrDefault();
        }

        public async Task<bool> AddFamilyMemberAsync(int familyId, Guid userId, string? role = null, Guid? createdBy = null)
        {
            var payload = new { family_id = familyId, user_id = userId, role, created_by = createdBy };
            var res = await PostAndReturnAsync<FamilyMemberRow>(FamilyMembersTable, payload);
            return res is { Count: > 0 };
        }

        public async Task<bool> UpdateFamilyMemberAsync(int id, string? role = null)
        {
            var payload = new { role };
            var res = await PatchAndReturnAsync<FamilyMemberRow>(FamilyMembersTable, $"id=eq.{id}", payload);
            return res is { Count: > 0 };
        }

        public async Task<bool> DeleteFamilyMemberAsync(int id)
        {
            var res = await DeleteAndReturnAsync<FamilyMemberRow>(FamilyMembersTable, $"id=eq.{id}");
            return res is { Count: > 0 };
        }

        public class FamilyMemberRow
        {
            public int Id { get; set; }
            public int Family_Id { get; set; }
            public Guid User_Id { get; set; }
            public string? Role { get; set; }
            public Guid? Created_By { get; set; }
            public DateTime? Created_At { get; set; }
        }
        #endregion

        #region calendars & calendar_shares
        private const string CalendarsTable = "calendars";
        private const string CalendarSharesTable = "calendar_shares";

        public async Task<List<CalendarRow>> GetCalendarsByFamilyIdAsync(int familyId)
        {
            var url = $"/rest/v1/{CalendarsTable}?family_id=eq.{familyId}&select=id,family_id,name,created_by,created_at&order=id.asc";
            return await GetListAsync<CalendarRow>(url) ?? new();
        }

        public async Task<CalendarRow?> GetCalendarByIdAsync(int id)
        {
            var url = $"/rest/v1/{CalendarsTable}?id=eq.{id}&select=id,family_id,name,created_by,created_at";
            var list = await GetListAsync<CalendarRow>(url);
            return list?.FirstOrDefault();
        }

        public async Task<bool> AddCalendarAsync(int familyId, string name, Guid? createdBy = null)
        {
            var payload = new { family_id = familyId, name, created_by = createdBy };
            var res = await PostAndReturnAsync<CalendarRow>(CalendarsTable, payload);
            return res is { Count: > 0 };
        }

        public async Task<bool> UpdateCalendarAsync(int id, string name)
        {
            var payload = new { name };
            var res = await PatchAndReturnAsync<CalendarRow>(CalendarsTable, $"id=eq.{id}", payload);
            return res is { Count: > 0 };
        }

        public async Task<bool> DeleteCalendarAsync(int id)
        {
            var res = await DeleteAndReturnAsync<CalendarRow>(CalendarsTable, $"id=eq.{id}");
            return res is { Count: > 0 };
        }

        // calendar_shares
        public async Task<List<CalendarShareRow>> GetCalendarSharesByCalendarIdAsync(int calendarId)
        {
            var url = $"/rest/v1/{CalendarSharesTable}?calendar_id=eq.{calendarId}&select=id,calendar_id,user_id,permission,created_at&order=id.asc";
            return await GetListAsync<CalendarShareRow>(url) ?? new();
        }

        public async Task<bool> AddCalendarShareAsync(int calendarId, Guid userId, string permission)
        {
            var payload = new { calendar_id = calendarId, user_id = userId, permission };
            var res = await PostAndReturnAsync<CalendarShareRow>(CalendarSharesTable, payload);
            return res is { Count: > 0 };
        }

        public async Task<bool> DeleteCalendarShareAsync(int id)
        {
            var res = await DeleteAndReturnAsync<CalendarShareRow>(CalendarSharesTable, $"id=eq.{id}");
            return res is { Count: > 0 };
        }

        public class CalendarRow
        {
            public int Id { get; set; }
            public int Family_Id { get; set; }
            public string? Name { get; set; }
            public Guid? Created_By { get; set; }
            public DateTime? Created_At { get; set; }
        }

        public class CalendarShareRow
        {
            public int Id { get; set; }
            public int Calendar_Id { get; set; }
            public Guid User_Id { get; set; }
            public string? Permission { get; set; }
            public DateTime? Created_At { get; set; }
        }
        #endregion

        #region events
        private const string EventsTable = "events";

        public async Task<List<EventRow>> GetEventsByCalendarIdAsync(int calendarId)
        {
            var url = $"/rest/v1/{EventsTable}?calendar_id=eq.{calendarId}&select=id,calendar_id,title,description,start_at,end_at,created_by&order=start_at.asc";
            return await GetListAsync<EventRow>(url) ?? new();
        }

        public async Task<EventRow?> GetEventByIdAsync(int id)
        {
            var url = $"/rest/v1/{EventsTable}?id=eq.{id}&select=id,calendar_id,title,description,start_at,end_at,created_by";
            var list = await GetListAsync<EventRow>(url);
            return list?.FirstOrDefault();
        }

        public async Task<bool> AddEventAsync(int calendarId, string title, string? description, DateTime startAt, DateTime? endAt, Guid? createdBy = null)
        {
            var payload = new
            {
                calendar_id = calendarId,
                title,
                description,
                start_at = startAt,
                end_at = endAt,
                created_by = createdBy
            };
            var res = await PostAndReturnAsync<EventRow>(EventsTable, payload);
            return res is { Count: > 0 };
        }

        public async Task<bool> UpdateEventAsync(int id, string title, string? description, DateTime? startAt = null, DateTime? endAt = null)
        {
            var payload = new { title, description, start_at = startAt, end_at = endAt };
            var res = await PatchAndReturnAsync<EventRow>(EventsTable, $"id=eq.{id}", payload);
            return res is { Count: > 0 };
        }

        public async Task<bool> DeleteEventAsync(int id)
        {
            var res = await DeleteAndReturnAsync<EventRow>(EventsTable, $"id=eq.{id}");
            return res is { Count: > 0 };
        }

        public class EventRow
        {
            public int Id { get; set; }
            public int Calendar_Id { get; set; }
            public string? Title { get; set; }
            public string? Description { get; set; }
            public DateTime? Start_At { get; set; }
            public DateTime? End_At { get; set; }
            public Guid? Created_By { get; set; }
        }
        #endregion

        #region task_lists & tasks & task_assignments
        private const string TaskListsTable = "task_lists";
        private const string TasksTable = "tasks";
        private const string TaskAssignmentsTable = "task_assignments";

        // task lists
        public async Task<List<TaskListRow>> GetTaskListsByFamilyIdAsync(int familyId)
        {
            var url = $"/rest/v1/{TaskListsTable}?family_id=eq.{familyId}&select=id,family_id,name,created_at&order=id.asc";
            return await GetListAsync<TaskListRow>(url) ?? new();
        }

        public async Task<TaskListRow?> GetTaskListByIdAsync(int id)
        {
            var url = $"/rest/v1/{TaskListsTable}?id=eq.{id}&select=id,family_id,name,created_at";
            var list = await GetListAsync<TaskListRow>(url);
            return list?.FirstOrDefault();
        }

        public async Task<bool> AddTaskListAsync(int familyId, string name)
        {
            var payload = new { family_id = familyId, name };
            var res = await PostAndReturnAsync<TaskListRow>(TaskListsTable, payload);
            return res is { Count: > 0 };
        }

        public async Task<bool> UpdateTaskListAsync(int id, string name)
        {
            var payload = new { name };
            var res = await PatchAndReturnAsync<TaskListRow>(TaskListsTable, $"id=eq.{id}", payload);
            return res is { Count: > 0 };
        }

        public async Task<bool> DeleteTaskListAsync(int id)
        {
            var res = await DeleteAndReturnAsync<TaskListRow>(TaskListsTable, $"id=eq.{id}");
            return res is { Count: > 0 };
        }

        // tasks
        public async Task<List<TaskRow>> GetTasksByTaskListIdAsync(int taskListId)
        {
            var url = $"/rest/v1/{TasksTable}?task_list_id=eq.{taskListId}&select=id,task_list_id,title,details,due_at,priority,repeats,created_by,completed,completed_at&order=id.asc";
            return await GetListAsync<TaskRow>(url) ?? new();
        }

        public async Task<TaskRow?> GetTaskByIdAsync(int id)
        {
            var url = $"/rest/v1/{TasksTable}?id=eq.{id}&select=id,task_list_id,title,details,due_at,priority,repeats,created_by,completed,completed_at";
            var list = await GetListAsync<TaskRow>(url);
            return list?.FirstOrDefault();
        }

        public async Task<bool> AddTaskAsync(int taskListId, string title, string? details = null, DateTime? dueAt = null, int? priority = null, string? repeats = null, Guid? createdBy = null)
        {
            var payload = new
            {
                task_list_id = taskListId,
                title,
                details,
                due_at = dueAt,
                priority,
                repeats,
                created_by = createdBy
            };
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

        // task assignments
        public async Task<List<TaskAssignmentRow>> GetAssignmentsByTaskIdAsync(int taskId)
        {
            var url = $"/rest/v1/{TaskAssignmentsTable}?task_id=eq.{taskId}&select=id,task_id,user_id,assigned_by,assigned_at&order=id.asc";
            return await GetListAsync<TaskAssignmentRow>(url) ?? new();
        }

        public async Task<bool> AddTaskAssignmentAsync(int taskId, Guid userId, Guid? assignedBy = null)
        {
            var payload = new { task_id = taskId, user_id = userId, assigned_by = assignedBy };
            var res = await PostAndReturnAsync<TaskAssignmentRow>(TaskAssignmentsTable, payload);
            return res is { Count: > 0 };
        }

        public async Task<bool> DeleteTaskAssignmentAsync(int id)
        {
            var res = await DeleteAndReturnAsync<TaskAssignmentRow>(TaskAssignmentsTable, $"id=eq.{id}");
            return res is { Count: > 0 };
        }

        public class TaskListRow
        {
            public int Id { get; set; }
            public int Family_Id { get; set; }
            public string? Name { get; set; }
            public DateTime? Created_At { get; set; }
        }

        public class TaskRow
        {
            public int Id { get; set; }
            public int Task_List_Id { get; set; }
            public string? Title { get; set; }
            public string? Details { get; set; }
            public DateTime? Due_At { get; set; }
            public int? Priority { get; set; }
            public string? Repeats { get; set; }
            public Guid? Created_By { get; set; }
            public bool? Completed { get; set; }
            public DateTime? Completed_At { get; set; }
        }

        public class TaskAssignmentRow
        {
            public int Id { get; set; }
            public int Task_Id { get; set; }
            public Guid User_Id { get; set; }
            public Guid? Assigned_By { get; set; }
            public DateTime? Assigned_At { get; set; }
        }
        #endregion

        #region lists & list_items
        private const string ListsTable = "lists";
        private const string ListItemsTable = "list_items";

        public async Task<List<ListRow>> GetListsByFamilyIdAsync(int familyId)
        {
            var url = $"/rest/v1/{ListsTable}?family_id=eq.{familyId}&select=id,family_id,name,created_at&order=id.asc";
            return await GetListAsync<ListRow>(url) ?? new();
        }

        public async Task<ListRow?> GetListByIdAsync(int id)
        {
            var url = $"/rest/v1/{ListsTable}?id=eq.{id}&select=id,family_id,name,created_at";
            var list = await GetListAsync<ListRow>(url);
            return list?.FirstOrDefault();
        }

        public async Task<bool> AddListAsync(int familyId, string name)
        {
            var payload = new { family_id = familyId, name };
            var res = await PostAndReturnAsync<ListRow>(ListsTable, payload);
            return res is { Count: > 0 };
        }

        public async Task<bool> UpdateListAsync(int id, string name)
        {
            var payload = new { name };
            var res = await PatchAndReturnAsync<ListRow>(ListsTable, $"id=eq.{id}", payload);
            return res is { Count: > 0 };
        }

        public async Task<bool> DeleteListAsync(int id)
        {
            var res = await DeleteAndReturnAsync<ListRow>(ListsTable, $"id=eq.{id}");
            return res is { Count: > 0 };
        }

        // list_items
        public async Task<List<ListItemRow>> GetListItemsByListIdAsync(int listId)
        {
            var url = $"/rest/v1/{ListItemsTable}?list_id=eq.{listId}&select=id,list_id,content,quantity,checked,created_by,created_at&order=id.asc";
            return await GetListAsync<ListItemRow>(url) ?? new();
        }

        public async Task<bool> AddListItemAsync(int listId, string content, string? quantity = null, Guid? createdBy = null)
        {
            var payload = new { list_id = listId, content, quantity, created_by = createdBy };
            var res = await PostAndReturnAsync<ListItemRow>(ListItemsTable, payload);
            return res is { Count: > 0 };
        }

        public async Task<bool> UpdateListItemAsync(int id, string? content = null, bool? checkedFlag = null)
        {
            var payload = new { content, @checked = checkedFlag };
            var res = await PatchAndReturnAsync<ListItemRow>(ListItemsTable, $"id=eq.{id}", payload);
            return res is { Count: > 0 };
        }

        public async Task<bool> DeleteListItemAsync(int id)
        {
            var res = await DeleteAndReturnAsync<ListItemRow>(ListItemsTable, $"id=eq.{id}");
            return res is { Count: > 0 };
        }

        public class ListRow
        {
            public int Id { get; set; }
            public int Family_Id { get; set; }
            public string? Name { get; set; }
            public DateTime? Created_At { get; set; }
        }

        public class ListItemRow
        {
            public int Id { get; set; }
            public int List_Id { get; set; }
            public string? Content { get; set; }
            public string? Quantity { get; set; }
            public bool? Checked { get; set; }
            public Guid? Created_By { get; set; }
            public DateTime? Created_At { get; set; }
        }
        #endregion

        #region notifications
        private const string NotificationsTable = "notifications";

        public async Task<List<NotificationRow>> GetNotificationsByUserIdAsync(Guid userId)
        {
            var url = $"/rest/v1/{NotificationsTable}?user_id=eq.{userId}&select=id,user_id,type,payload,read,created_at&order=id.desc";
            return await GetListAsync<NotificationRow>(url) ?? new();
        }

        public async Task<bool> AddNotificationAsync(Guid userId, string type, object? payload = null)
        {
            var payloadObj = new { user_id = userId, type, payload = payload == null ? null : JsonSerializer.Serialize(payload), read = false };
            var res = await PostAndReturnAsync<NotificationRow>(NotificationsTable, payloadObj);
            return res is { Count: > 0 };
        }

        public async Task<bool> MarkNotificationReadAsync(int id)
        {
            var payload = new { read = true };
            var res = await PatchAndReturnAsync<NotificationRow>(NotificationsTable, $"id=eq.{id}", payload);
            return res is { Count: > 0 };
        }

        public async Task<bool> DeleteNotificationAsync(int id)
        {
            var res = await DeleteAndReturnAsync<NotificationRow>(NotificationsTable, $"id=eq.{id}");
            return res is { Count: > 0 };
        }

        public class NotificationRow
        {
            public int Id { get; set; }
            public Guid User_Id { get; set; }
            public string? Type { get; set; }
            public string? Payload { get; set; }
            public bool? Read { get; set; }
            public DateTime? Created_At { get; set; }
        }
        #endregion
    }
}
