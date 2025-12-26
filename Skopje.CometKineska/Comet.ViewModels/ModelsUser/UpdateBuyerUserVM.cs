namespace Comet.ViewModels.ModelsUser
{
    public class UpdateBuyerUserVM
    {
        public string CompanyName { get; set; }
        public string ContactPerson { get; set; }
        public string PhoneNumber { get; set; }
        public string ShippingAddress { get; set; }
        public decimal? CreditLimit { get; set; }
        public bool? IsActive { get; set; }
    }
}
