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
    }

}
