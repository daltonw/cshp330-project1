using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MiniCstructor.Website.Models
{
    public class EnrollInClassModel
    {
        public int UserId  { get; set; }
        public int ClassId { get; set; }
        public string ClassName { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
    }
}
