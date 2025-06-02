using BOs.Data;
using BOs.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAOs
{
    public class BlogDAO
    {
        private static BlogDAO instance = null;
        private readonly DataContext _context;

        private BlogDAO()
        {
            _context = new DataContext();
        }

        public static BlogDAO Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new BlogDAO();
                }
                return instance;
            }
        }

        public async Task<bool> CreateBlogAsync(Blog blog)
        {
            if (string.IsNullOrWhiteSpace(blog.Title) || string.IsNullOrWhiteSpace(blog.Content))
                return false;

            // Validate CategoryID
            var categoryExists = await _context.Categories.AnyAsync(c => c.CategoryID == blog.CategoryID);
            if (!categoryExists)
                return false;

            blog.Title = blog.Title.Trim();
            blog.Description = blog.Description?.Trim();
            blog.Content = blog.Content.Trim();

            // Không gọi Trim với byte[]
            // blog.Image giữ nguyên

            await _context.Blogs.AddAsync(blog);
            return await _context.SaveChangesAsync() > 0;
        }


        public async Task<bool> UpdateBlogAsync(Blog blog)
        {
            var existing = await _context.Blogs.FirstOrDefaultAsync(b => b.BlogID == blog.BlogID);
            if (existing == null)
                return false;

            bool hasChanges = false;

            if (!string.IsNullOrWhiteSpace(blog.Title) && blog.Title.Trim() != existing.Title)
            {
                existing.Title = blog.Title.Trim();
                hasChanges = true;
            }

            if (blog.Description != null && blog.Description.Trim() != existing.Description)
            {
                existing.Description = blog.Description.Trim();
                hasChanges = true;
            }

            if (!string.IsNullOrWhiteSpace(blog.Content) && blog.Content.Trim() != existing.Content)
            {
                existing.Content = blog.Content.Trim();
                hasChanges = true;
            }

            // So sánh mảng byte của ảnh:
            if (blog.Image != null && !AreByteArraysEqual(blog.Image, existing.Image))
            {
                existing.Image = blog.Image;
                hasChanges = true;
            }

            if (blog.CategoryID != existing.CategoryID)
            {
                existing.CategoryID = blog.CategoryID;
                hasChanges = true;
            }

            if (hasChanges)
            {
                _context.Blogs.Update(existing);
                return await _context.SaveChangesAsync() > 0;
            }

            return false;
        }

        // Hàm so sánh 2 mảng byte
        private bool AreByteArraysEqual(byte[]? a1, byte[]? a2)
        {
            if (ReferenceEquals(a1, a2))
                return true;
            if (a1 == null || a2 == null)
                return false;
            if (a1.Length != a2.Length)
                return false;
            for (int i = 0; i < a1.Length; i++)
            {
                if (a1[i] != a2[i]) return false;
            }
            return true;
        }


        public async Task<List<Blog>> GetAllBlogsAsync()
        {
            return await _context.Blogs
                .Include(b => b.Account)
                .Include(b => b.Category)
                .AsNoTracking()
                .ToListAsync();
        }


        public async Task<Blog?> GetBlogByIdAsync(int blogId)
        {
            return await _context.Blogs
                .Include(b => b.Account)
                .Include(b => b.Category)
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.BlogID == blogId);
        }

        public async Task<bool> DeleteBlogAsync(int blogId)
        {
            var blog = await _context.Blogs.FirstOrDefaultAsync(b => b.BlogID == blogId);
            if (blog == null)
                return false;

            _context.Blogs.Remove(blog);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}


