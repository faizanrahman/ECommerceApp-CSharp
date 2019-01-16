using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ECommerceApp.Models;
using Microsoft.EntityFrameworkCore;

namespace ECommerceApp.Data
{
    public class Repository : IRepository
    {
        private readonly DataContext _context;

        public Repository(DataContext context)
        {
            this._context = context;
        }

        public IEnumerable<Product> AllProducts()
        {
            var allproducts = _context.Products.ToList();
            return allproducts;
            // throw new System.NotImplementedException();
        }

        public Product GetProductById(int id)
        {
            var product = _context.Products.SingleOrDefault(p=>p.ProductId == id);
            return product;
        }

        public void CreateProduct(Product product)
        {
            Product newProduct = new Product
            {
                ProductName = product.ProductName,
                ProductDescription = product.ProductDescription,
                InitialQuantity = product.InitialQuantity,
                Price = product.Price,
                ImageUrl=product.ImageUrl,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
            };
        }

        public IEnumerable<OrderDetail> OrderDetailsByOrderId(int OrderId)
        {
            var orderdetail = _context.OrderDetails
            .Include(o=> o.productOrdered)
            .Include(p=>p.order)
            .Where(x=>x.OrderId == OrderId)
            .ToList();

            return orderdetail;
        }

        public IEnumerable<Order> OrdersByUser(int userId)
        {
            var allorders = _context.Orders
                .Include(o=>o.Creator)
                .Where(u=> u.UserID == userId) 
                .OrderByDescending(x=>x.OrderDate)
                .ToList();

            return allorders;
        }


        // Cart Controller Methods
        public IEnumerable<Item> GetCartItemsByUser(int userId)
        {
            var cart = _context.Items.Include(p => p.Product).Include(u => u.User)
                .Where(u => u.UserID == userId).ToList();
            
            return cart;
        }

        public Item CheckItemInCart(int userId, int productId)
        {
            Item checkItem = _context.Items
                    .Include (i => i.Product)
                    .Include (p => p.User)
                    .SingleOrDefault (u => u.UserID == userId && u.ProductId == productId);
            
            return checkItem;
        }

        public Item removedItem(int itemId)
        {
            Item removedItem = _context.Items.SingleOrDefault (i => i.ItemId == itemId);

            return removedItem;
        }

        public Order CreateOrder(int userId, float orderTotal)
        {
            Order newOrder = new Order
            {
                OrderDate = DateTime.Now,
                UpdatedAt = DateTime.Now,
                UserID = userId,
                OrderTotal = orderTotal,
            };

            return newOrder;
        }

        public OrderDetail CreateOrderDetail(int orderId, int productId, float price, int quantity, string productName, float subTotal)
        {
            OrderDetail orderDetail = new OrderDetail
            {
                OrderId = orderId,
                ProductId = productId,
                Price = price,
                Quantity = quantity,
                ProductName = productName,
                SubTotal = subTotal,
            };

            return orderDetail;
        }
        
    }
}