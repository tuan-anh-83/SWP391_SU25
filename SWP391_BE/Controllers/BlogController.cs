namespace SWP391_BE.Controllers
{
    using BOs.Models;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Services;
    using SWP391_BE.DTO;
    using System.ComponentModel.DataAnnotations;
    using System.Security.Claims;

    [ApiController]
    [Route("api/[controller]")]
    public class BlogController : ControllerBase
    {
        private readonly IBlogService _blogService;
        private readonly IAccountService _accountService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public BlogController(IBlogService blogService, IAccountService accountService, IHttpContextAccessor httpContextAccessor)
        {
            _blogService = blogService;
            _accountService = accountService;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet("GetAllBlogs")]
        public async Task<IActionResult> GetAllBlogs()
        {
            var blogs = await _blogService.GetAllBlogsAsync();

            var result = blogs.Select(b => new
            {
                b.BlogID,
                b.Title,
                b.Description,
                b.Content,
                b.Image,
                b.CategoryID,
                CategoryName = b.Category?.Name,
                b.AccountID,
                AuthorName = b.Account?.Fullname,
            });

            return Ok(result);
        }

        [HttpGet("GetBlogById/{id}")]
        public async Task<IActionResult> GetBlogById(int id)
        {
            var blog = await _blogService.GetBlogByIdAsync(id);
            if (blog == null) return NotFound(new { message = "Blog not found." });

            var result = new
            {
                blog.BlogID,
                blog.Title,
                blog.Description,
                blog.Content,
                blog.Image,
                blog.CategoryID,
                CategoryName = blog.Category?.Name,
                blog.AccountID,
                AuthorName = blog.Account?.Fullname,
            };

            return Ok(result);
        }

        [HttpPost("CreateBlog")]
        [Authorize(Roles = "Nurse,Admin")]
        public async Task<IActionResult> CreateBlog([FromForm] BlogCreateModelDTO model)
        {
            var accountIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (accountIdClaim == null || !int.TryParse(accountIdClaim.Value, out int accountId))
                return Unauthorized(new { message = "Invalid or missing token." });

            var account = await _accountService.GetAccountByIdAsync(accountId);
            if (account == null)
                return NotFound(new { message = "Account not found." });
            if (!account.Status.Equals("Active", StringComparison.OrdinalIgnoreCase))
                return Unauthorized(new { message = "Account is not active." });

            byte[]? imageData = null;
            if (model.Image != null && model.Image.Length > 0)
            {
                using var ms = new MemoryStream();
                await model.Image.CopyToAsync(ms);
                imageData = ms.ToArray();
            }

            var blog = new Blog
            {
                Title = model.Title.Trim(),
                Description = model.Description?.Trim(),
                Content = model.Content.Trim(),
                Image = imageData,
                CategoryID = model.CategoryID,
                AccountID = account.AccountID
            };

            var created = await _blogService.CreateBlogAsync(blog);
            if (!created)
                return BadRequest(new { message = "Failed to create blog." });

            return CreatedAtAction(nameof(GetBlogById), new { id = blog.BlogID }, new { message = "Blog created successfully.", blogId = blog.BlogID });
        }

        [HttpPut("UpdateBlog/{id}")]
        [Authorize(Roles = "Nurse,Admin")]
        public async Task<IActionResult> UpdateBlog(int id, [FromForm] BlogUpdateModelDTO model)
        {
            var blog = await _blogService.GetBlogByIdAsync(id);
            if (blog == null) return NotFound(new { message = "Blog not found." });

            var accountIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (accountIdClaim == null || !int.TryParse(accountIdClaim.Value, out int accountId))
                return Unauthorized(new { message = "Invalid or missing token." });

            var account = await _accountService.GetAccountByIdAsync(accountId);
            if (account == null)
                return NotFound(new { message = "Account not found." });
            if (!account.Status.Equals("Active", StringComparison.OrdinalIgnoreCase))
                return Unauthorized(new { message = "Account is not active." });

            blog.Title = model.Title?.Trim() ?? blog.Title;
            blog.Description = model.Description?.Trim() ?? blog.Description;
            blog.Content = model.Content?.Trim() ?? blog.Content;
            blog.CategoryID = model.CategoryID ?? blog.CategoryID;

            if (model.Image != null && model.Image.Length > 0)
            {
                using var ms = new MemoryStream();
                await model.Image.CopyToAsync(ms);
                blog.Image = ms.ToArray();
            }

            var updated = await _blogService.UpdateBlogAsync(blog);
            if (!updated)
                return BadRequest(new { message = "Failed to update blog." });

            return Ok(new { message = "Blog updated successfully." });
        }

        [HttpDelete("DeleteBlog/{id}")]
        [Authorize(Roles = "Nurse,Admin")]
        public async Task<IActionResult> DeleteBlog(int id)
        {
            var blog = await _blogService.GetBlogByIdAsync(id);
            if (blog == null) return NotFound(new { message = "Blog not found." });

            var accountIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (accountIdClaim == null || !int.TryParse(accountIdClaim.Value, out int accountId))
                return Unauthorized(new { message = "Invalid or missing token." });

            var account = await _accountService.GetAccountByIdAsync(accountId);
            if (account == null)
                return NotFound(new { message = "Account not found." });
            if (!account.Status.Equals("Active", StringComparison.OrdinalIgnoreCase))
                return Unauthorized(new { message = "Account is not active." });

            var deleted = await _blogService.DeleteBlogAsync(id);
            if (!deleted)
                return BadRequest(new { message = "Failed to delete blog." });

            return Ok(new { message = "Blog deleted successfully." });
        }
    }
}
