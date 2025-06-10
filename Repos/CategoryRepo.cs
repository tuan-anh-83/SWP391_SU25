using BOs.Models;
using DAOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repos
{
    public class CategoryRepo : ICategoryRepo
    {
        public async Task<List<Category>> GetAllCategoriesAsync()
        {
            return await CategoryDAO.Instance.GetAllCategoriesAsync();
        }

        public async Task<Category?> GetCategoryByIdAsync(int id)
        {
            return await CategoryDAO.Instance.GetCategoryByIdAsync(id);
        }

        public async Task<Category> CreateCategoryAsync(Category category)
        {
            return await CategoryDAO.Instance.CreateCategoryAsync(category);
        }

        public async Task<bool> UpdateCategoryAsync(Category category)
        {
            return await CategoryDAO.Instance.UpdateCategoryAsync(category);
        }

        public async Task<bool> DeleteCategoryAsync(int id)
        {
            return await CategoryDAO.Instance.DeleteCategoryAsync(id);
        }
    }

}
