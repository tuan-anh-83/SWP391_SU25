namespace SWP391_BE.DTO
{
    public class StudentDistributionDTO
    {
        public Dictionary<string, int> ByGender { get; set; }
        public Dictionary<string, int> ByAge { get; set; }
        public List<ClassDistributionDTO> ByClass { get; set; }
    }

    public class ClassDistributionDTO
    {
        public string ClassName { get; set; }
        public int Count { get; set; }
    }
}