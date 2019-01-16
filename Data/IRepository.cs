using System.Collections;
using System.Collections.Generic;
using ECommerceApp.Models;

namespace ECommerceApp.Data
{
    public interface IRepository
    {
        IEnumerable<Product> AllProducts();

        Product GetProductById(int id);

        void CreateProduct(Product product);

        IEnumerable<OrderDetail> OrderDetailsByOrderId(int id);

        IEnumerable<Order> OrdersByUser(int userId);

        // Cart Controller Methods

        IEnumerable<Item> GetCartItemsByUser(int userId);

        Item CheckItemInCart(int userId, int productId);

        Item removedItem(int itemId);

        Order CreateOrder(int userId, float orderTotal);

        OrderDetail CreateOrderDetail(int orderId, int productId, float price, int quantity,
         string productName, float subTotal);
    }
}