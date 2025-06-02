using BOs.Models;
using DAOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repos
{
    public class BlogRepo : IBlogRepo
    {
        public async Task<List<Blog>> GetAllBlogsAsync()
        {
            return await BlogDAO.Instance.GetAllBlogsAsync();
        }

        public async Task<Blog?> GetBlogByIdAsync(int blogId)
        {
            return await BlogDAO.Instance.GetBlogByIdAsync(blogId);
        }

        public async Task<bool> CreateBlogAsync(Blog blog)
        {
            return await BlogDAO.Instance.CreateBlogAsync(blog);
        }

        public async Task<bool> UpdateBlogAsync(Blog blog)
        {
            return await BlogDAO.Instance.UpdateBlogAsync(blog);
        }

        public async Task<bool> DeleteBlogAsync(int blogId)
        {
            return await BlogDAO.Instance.DeleteBlogAsync(blogId);
        }
    }

}
