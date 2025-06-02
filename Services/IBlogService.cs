using BOs.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface IBlogService
    {
        Task<List<Blog>> GetAllBlogsAsync();
        Task<Blog?> GetBlogByIdAsync(int blogId);
        Task<bool> CreateBlogAsync(Blog blog);
        Task<bool> UpdateBlogAsync(Blog blog);
        Task<bool> DeleteBlogAsync(int blogId);
    }

}
