using System.ComponentModel.DataAnnotations;

namespace SWP391_BE.DTO
{
    public class BlogCreateModelDTO
    {
            [Required]
            public string Title { get; set; }

            public string? Description { get; set; }

            [Required]
            public string Content { get; set; }

            public IFormFile? Image { get; set; }

            [Required]
            public int CategoryID { get; set; }
        
    }
}
