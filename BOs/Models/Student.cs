using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BOs.Models
{
    public class Student
    {
        public int StudentId { get; set; }
        public string Fullname { get; set; }
        public int ClassId { get; set; }
        public string StudentCode { get; set; }
        public string Gender { get; set; }
        public int? ParentId { get; set; }
        public DateTime DateOfBirth { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdateAt { get; set; }
        
        public Account Parent { get; set; }
        public Class Class { get; set; }
    }


    public enum Gender
    {
        Male = 0,
        Female = 1,
    }

}
