using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models
{
    public class ListItemRow
    {
        public int Id { get; set; }
        public int List_Id { get; set; }
        public string? Content { get; set; }
        public string? Quantity { get; set; }
        public bool? Checked { get; set; }
        public Guid? Created_By { get; set; }
        public DateTime? Created_At { get; set; }
    }
}
