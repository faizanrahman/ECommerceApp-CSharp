using System;
using System.Collections.Generic;

namespace ECommerceApp.Models
{
    public class Order
    {
        public int OrderId {get;set;}
        public int OrderQunatity {get;set;}
        public DateTime OrderDate {get;set;}
        public DateTime UpdatedAt {get;set;}
        public int UserID {get;set;}
        public User Creator {get;set;}
        public List<OrderDetail> OrderDetails {get;set;}    

    }
}