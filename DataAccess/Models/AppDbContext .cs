using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DataAccess.Models;

namespace DataAccess.Models
{
     public class AppDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

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
