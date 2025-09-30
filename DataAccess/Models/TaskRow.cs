using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models
{
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
}
