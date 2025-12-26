namespace Comet.ViewModels.ModelsUser
{
    public class BuyerUserVM : UserVM
    {
        public string CompanyName { get; set; }
        public string ContactPerson { get; set; }
        public override bool CanSubmitPrice() => true;
    }
}
