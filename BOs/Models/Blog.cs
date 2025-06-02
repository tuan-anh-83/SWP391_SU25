using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BOs.Models
{
    public class Blog
    {
        public int BlogID { get; set; }
        public int AccountID { get; set; }
        public int CategoryID { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }
        public string Content { get; set; }

        public byte[]? Image { get; set; } 

        public Account Account { get; set; }
        public Category Category { get; set; }
    }

}
