namespace Comet.DTO.DTOs
{
    public class ImportResultDto
    {
        public int TotalRows { get; set; }
        public int Inserted { get; set; }
        public int Updated { get; set; }
        public List<string> Errors { get; set; } = new();
    }
}
