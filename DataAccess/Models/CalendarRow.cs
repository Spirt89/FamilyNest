using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models
{
    public class CalendarRow
    {
        public int Id { get; set; }
        public int Family_Id { get; set; }
        public string? Name { get; set; }
        public Guid? Created_By { get; set; }
        public DateTime? Created_At { get; set; }
    }
}
