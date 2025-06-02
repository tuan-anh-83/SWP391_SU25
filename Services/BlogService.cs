using BOs.Models;
using Repos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class BlogService : IBlogService
    {
        private readonly IBlogRepo _blogRepo;

        public BlogService(IBlogRepo blogRepo)
        {
            _blogRepo = blogRepo;
        }

        public async Task<List<Blog>> GetAllBlogsAsync()
        {
            return await _blogRepo.GetAllBlogsAsync();
        }

        public async Task<Blog?> GetBlogByIdAsync(int blogId)
        {
            return await _blogRepo.GetBlogByIdAsync(blogId);
        }

        public async Task<bool> CreateBlogAsync(Blog blog)
        {
            return await _blogRepo.CreateBlogAsync(blog);
        }

        public async Task<bool> UpdateBlogAsync(Blog blog)
        {
            return await _blogRepo.UpdateBlogAsync(blog);
        }

        public async Task<bool> DeleteBlogAsync(int blogId)
        {
            return await _blogRepo.DeleteBlogAsync(blogId);
        }
    }

}
