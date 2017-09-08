using System;
namespace CoffeeShop.Web
{
	public class Order
	{
		public Order (string slug)
		{
			if (slug == "large") {
				this.Price = 3.99M;
				this.ProductName = "Large Coffee";
			} else if (slug == "medium") {
				this.Price = 2.99M;
				this.ProductName = "Medium Coffee";
			} else {
				this.Price = 1.99M;
				this.ProductName = "Small Coffee";			
			}
			this.Slug = slug;
            this.OrderID = Guid.NewGuid();
            
		}
        public string AuthCode { get; set; }
        public string TransactionID { get; set; }
        public string OrderMessage { get; set; }
        public Guid OrderID { get; set; }
		public string ProductName { get; set; }
		public decimal Price { get; set; }
		public string Slug {
			get;
			set;
		}
	}
}

