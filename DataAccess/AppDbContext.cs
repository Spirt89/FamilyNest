using Microsoft.EntityFrameworkCore;
using DataAccess.Models;

namespace DataAccess
{
    // Должен быть public, а не internal
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // Таблицы
        public DbSet<FamilyRow> Families { get; set; }
        public DbSet<ProfileRow> Profiles { get; set; }
        public DbSet<FamilyMemberRow> FamilyMembers { get; set; }
        public DbSet<CalendarRow> Calendars { get; set; }
        public DbSet<CalendarShareRow> CalendarShares { get; set; }
        public DbSet<EventRow> Events { get; set; }
        public DbSet<TaskListRow> TaskLists { get; set; }
        public DbSet<TaskRow> Tasks { get; set; }
        public DbSet<TaskAssignmentRow> TaskAssignments { get; set; }
        public DbSet<ListRow> Lists { get; set; }
        public DbSet<ListItemRow> ListItems { get; set; }
        public DbSet<NotificationRow> Notifications { get; set; }
    }
}
