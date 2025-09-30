using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models
{
    public class TaskAssignmentRow
    {
        public int Id { get; set; }
        public int Task_Id { get; set; }
        public Guid User_Id { get; set; }
        public Guid? Assigned_By { get; set; }
        public DateTime? Assigned_At { get; set; }
    }
}
