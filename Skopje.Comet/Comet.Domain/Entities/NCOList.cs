using Comet.Domain.Enums;

namespace Comet.Domain.Entities
{
    public class NCOList
    {
        public int Id { get; set; }
        public string BuyerUserId { get; set; } = string.Empty;
        public string BuyerUserName { get; set; } = string.Empty; // BUyer Company Name
        public Incoterm Incoterm { get; set; }
        public int Price { get; set; }
        public string ProductCode { get; set; } = string.Empty;
    }
}
