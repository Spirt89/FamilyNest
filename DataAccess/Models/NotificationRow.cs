using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models
{
    public class NotificationRow
    {
        public int Id { get; set; }
        public Guid User_Id { get; set; }
        public string? Type { get; set; }
        public string? Payload { get; set; }
        public bool? Read { get; set; }
        public DateTime? Created_At { get; set; }
    }
}
