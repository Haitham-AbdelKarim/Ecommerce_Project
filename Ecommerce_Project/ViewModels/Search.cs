namespace Ecommerce_Project.ViewModels
{
    public class Search
    {
        public string? Name { get; set; }
        public int? MinPrice { get; set; }

        public int? MaxPrice { get; set; }

        public int CategoryId { get; set; }
    }
}
