using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models
{
    public class ProfileRow
    {
        public Guid Id { get; set; }
        public string? Full_Name { get; set; }
        public string? Email { get; set; }
        public string? Avatar_Url { get; set; }
        public DateTime? Updated_At { get; set; }
    }
}
