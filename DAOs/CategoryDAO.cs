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
    public class CategoryDAO
    {
        private static CategoryDAO instance = null;
        private readonly DataContext _context;

        private CategoryDAO()
        {
            _context = new DataContext();
        }

        public static CategoryDAO Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new CategoryDAO();
                }
                return instance;
            }
        }

        public async Task<List<Category>> GetAllCategoriesAsync()
        {
            return await _context.Categories.ToListAsync();
        }

        public async Task<Category?> GetCategoryByIdAsync(int id)
        {
            return await _context.Categories
                                 .Include(c => c.Blogs)
                                 .FirstOrDefaultAsync(c => c.CategoryID == id);
        }
    }

}
