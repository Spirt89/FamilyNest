using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models
{
    public class ListRow
    {
        public int Id { get; set; }
        public int Family_Id { get; set; }
        public string? Name { get; set; }
        public DateTime? Created_At { get; set; }
    }
}
