using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models
{
    public class FamilyRow
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public Guid? Created_By { get; set; }
        public DateTime? Created_At { get; set; }
    }
}
