using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ECommerceApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using ECommerceApp.Helpers;
using ECommerceApp.Data;
using Microsoft.EntityFrameworkCore;

namespace ECommerceApp.Controllers
{
    

    public class CartController : Controller
    {

        private readonly DataContext _context;

        public CartController(DataContext context)
        {
            this._context = context;
        }


        [Route("cart")]
        public IActionResult Index()
        {
            int? userId = HttpContext.Session.GetInt32("ID");
            // var cart = SessionHelper.GetObjectFromJson<List<Item>>(HttpContext.Session, "cart");
            var cart = _context.Items.Include(p=> p.Product).Include(u=> u.User)
            .Where(u=>u.UserID == userId).ToList();
            System.Console.WriteLine("$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$");
            // foreach(var item in cart)
            // {
            //     System.Console.WriteLine(item.Product.ProductId);
            //     System.Console.WriteLine(item.Product.ProductName);
            //     System.Console.WriteLine(item.Product.Price);
            //     System.Console.WriteLine(item);
            //     // System.Console.WriteLine(item.User.FirstName);


            // }
            
            System.Console.WriteLine("$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$");
            ViewBag.cart = cart;
            ViewBag.total = cart.Sum(item => item.Product.Price * item.Quantity);
            return View();
        }

        [HttpGet("addtocart/{id}")]
        public IActionResult Buy(int id)
        {

            int? userId = HttpContext.Session.GetInt32("ID");
           
            // ProductModel productModel = new ProductModel();
            if (SessionHelper.GetObjectFromJson<List<Item>>(HttpContext.Session, "cart") == null)
            {
                Item cart = new Item();
                // cart.Add(new Item { Product = productModel.find(id), Quantity = 1 });
                cart.Product = _context.Products.SingleOrDefault(p=>p.ProductId == id);
                cart.Quantity = 1;
                cart.User = _context.Users.SingleOrDefault(u=>u.UserID == userId);
                cart.UserID = (int)userId;
                _context.Add(cart);
                _context.SaveChanges();

                SessionHelper.SetObjectAsJson(HttpContext.Session, "cart", cart);
            }
            else
            {
                List<Item> cart = SessionHelper.GetObjectFromJson<List<Item>>(HttpContext.Session, "cart");
                int index = isExist(id);
                if (index != -1)
                {
                    cart[index].Quantity++;
                }
                else
                {
                    // cart.Add(new Item { Product = productModel.find(id), Quantity = 1 });
                    cart.Add(new Item 
                    { 
                        Product = _context.Products.SingleOrDefault(p=>p.ProductId == id),
                        Quantity = 1,
                        UserID = (int)userId,
                        User = _context.Users.SingleOrDefault(u=>u.UserID == userId),
                    });
                }
                SessionHelper.SetObjectAsJson(HttpContext.Session, "cart", cart);
            }
            return RedirectToAction("Index");
        }

        [Route("remove/{id}")]
        public IActionResult Remove(int id)
        {
            // List<Item> cart = SessionHelper.GetObjectFromJson<List<Item>>(HttpContext.Session, "cart");
            var removedItem = _context.Items.SingleOrDefault(i=>i.ItemId == id);
            _context.Remove(removedItem);
            _context.SaveChanges();
            // int index = isExist(id);
            //removedItem.RemoveAt(index);
            //SessionHelper.SetObjectAsJson(HttpContext.Session, "cart", cart);
            return RedirectToAction("Index");
        }

        private int isExist(int id)
        {
            List<Item> cart = SessionHelper.GetObjectFromJson<List<Item>>(HttpContext.Session, "cart");
            for (int i = 0; i < cart.Count; i++)
            {
                if (cart[i].Product.ProductId.Equals(id))
                {
                    return i;
                }
            }
            return -1;
        }

    }   
}