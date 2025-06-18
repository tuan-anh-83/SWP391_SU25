namespace SWP391_BE.DTO
{
    public class BlogUpdateModelDTO
    {
            public string? Title { get; set; }
            public string? Description { get; set; }
            public string? Content { get; set; }
            public IFormFile? Image { get; set; }
            public int? CategoryID { get; set; }
        
    }
}
