namespace ECommerceApp.Models
{
    public class OrderDetail
    {
        public int OrderDetailId {get;set;}
        public int OrderId {get;set;}
        public int ProductId {get;set;}
        public Order order {get;set;} 
        public Product productOrdered {get;set;}
 
    }
}