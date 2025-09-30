using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models
{
    public class CalendarShareRow
    {
        public int Id { get; set; }
        public int Calendar_Id { get; set; }
        public Guid User_Id { get; set; }
        public string? Permission { get; set; }
        public DateTime? Created_At { get; set; }
    }
}
