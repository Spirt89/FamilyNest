using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models
{
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
}
