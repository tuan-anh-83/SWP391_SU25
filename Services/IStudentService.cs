using BOs.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface IStudentService
    {
        Task<Student?> GetStudentByCodeAsync(string studentCode);
        Task<bool> LinkStudentToParentAsync(string studentCode, int parentId);
    }

}
