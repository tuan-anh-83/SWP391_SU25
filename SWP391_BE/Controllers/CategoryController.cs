using Microsoft.AspNetCore.Mvc;
using Services;
using BOs.Models;
using SWP391_BE.DTO;

namespace SWP391_BE.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet("GetAllCategory")]
        public async Task<IActionResult> GetAll()
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            return Ok(categories);
        }

        [HttpGet("GetCateById{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category == null)
                return NotFound("Category not found.");
            return Ok(category);
        }

        [HttpPost("CreateCategory")]
        public async Task<IActionResult> CreateCategory([FromBody] CategoryCreateDTO dto)
        {
            var newCategory = new Category { Name = dto.Name };
            var created = await _categoryService.CreateCategoryAsync(newCategory);
            return Ok(new
            {
                message = "Category created successfully.",
                data = created
            });
        }

        [HttpPut("UpdateCategory/{id}")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] CategoryUpdateDTO dto)
        {
            var updateCategory = new Category { CategoryID = id, Name = dto.Name };
            var result = await _categoryService.UpdateCategoryAsync(updateCategory);
            if (!result)
                return NotFound(new { message = "Category not found." });

            return Ok(new { message = "Category updated successfully." });
        }

        [HttpDelete("DeleteCategory/{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var result = await _categoryService.DeleteCategoryAsync(id);
            if (!result)
                return NotFound(new { message = "Category not found." });

            return Ok(new { message = "Category deleted successfully." });
        }
    }
}
