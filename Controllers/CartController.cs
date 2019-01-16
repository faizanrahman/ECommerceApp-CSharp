using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using ECommerceApp.Data;
using ECommerceApp.Helpers;
using ECommerceApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Stripe;

namespace ECommerceApp.Controllers {

    public class CartController : Controller {

        private readonly DataContext _context;
        private readonly IRepository _repo;

        public CartController (DataContext context, IRepository repo) 
        {
            this._repo = repo;
            this._context = context;
        }

        [Route ("cart")]
        public IActionResult Index () {
            int? userId = HttpContext.Session.GetInt32 ("ID");

            var cart = _repo.GetCartItemsByUser((int)userId);

            System.Console.WriteLine ("$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$");
            
            System.Console.WriteLine ("$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$");
            ViewBag.cart = cart;
            ViewBag.total = (float) cart.Sum (item => item.Product.Price * item.Quantity);
            float before = cart.Sum (item => item.Product.Price * item.Quantity) * 100;
            int total = (int) before;
            HttpContext.Session.SetInt32 ("total", total);
            return View ();
        }

        public IActionResult Error () {
            return View ();
        }

        [HttpGet ("addtocart/{id}")]
        public IActionResult Buy (int id) {

            int? userId = HttpContext.Session.GetInt32 ("ID");

            var thisproduct = _repo.GetProductById(id);
            if (thisproduct.InitialQuantity > 0) 
            {   
                Item checkItem = _repo.CheckItemInCart((int)userId, id);
                
                if (checkItem != null && checkItem.ProductId == id) 
                {
                    // if true - update quantity
                    checkItem.Quantity += 1;
                    _context.SaveChanges ();
                    return RedirectToAction ("Index");
                }
                // check to see if product id already exists in cart
                // if false create new new item
                else {
                    Item cart = new Item ();
                    // cart.Add(new Item { Product = productModel.find(id), Quantity = 1 });
                    cart.ProductId = id;
                    cart.Product = _context.Products.SingleOrDefault (p => p.ProductId == id);
                    cart.Quantity = 1;
                    cart.User = _context.Users.SingleOrDefault (u => u.UserID == userId);
                    cart.UserID = (int) userId;

                    _context.Add (cart);
                    _context.SaveChanges ();

                    return RedirectToAction ("Index");
                }

            }
            TempData["QtyError"] = "This item is out of stock. Please check back soon.";
            return RedirectToAction ("AllProducts", "Home");
        }

        [Route ("remove/{id}")]
        public IActionResult Remove (int id) {
           
            var removedItem = _repo.removedItem(id);
            _context.Remove (removedItem);
            _context.SaveChanges ();
           
            return RedirectToAction ("Index");
        }


        [HttpGet ("payments")]
        public IActionResult PaymentPage () 
        {
            int? userId = HttpContext.Session.GetInt32 ("ID");

            var allitems = _repo.GetCartItemsByUser((int)userId);
            ViewBag.allitems = allitems;
            ViewBag.amount = HttpContext.Session.GetInt32 ("total");

            var cart = _repo.GetCartItemsByUser((int)userId);

            ViewBag.total = (float) cart.Sum (item => item.Product.Price * item.Quantity);
            return View ();
        }

        [HttpGet ("success")]
        public IActionResult Success ()

        {
            return View ("Charge");
        }

        [HttpPost ("charge")]
        public IActionResult Charge (string stripeEmail, string stripeToken) 
        {

            int? userId = HttpContext.Session.GetInt32 ("ID");

            var customers = new CustomerService ();
            var charges = new ChargeService ();

            var customer = customers.Create (new CustomerCreateOptions {
                Email = stripeEmail,
                    SourceToken = stripeToken
            });

            StripeConfiguration.SetApiKey ("sk_test_cXmDQf2AysFtVCS1M1sfcBRC");

            var charge = charges.Create (new ChargeCreateOptions {
                Amount = HttpContext.Session.GetInt32 ("total"),
                    Description = "This is a test Charge",
                    Currency = "usd",
                    CustomerId = customer.Id
            });

            TempData["ChargeAmount"] = (charge.Amount / (double) 100).ToString ("N2");


            var allitems = _repo.GetCartItemsByUser((int)HttpContext.Session.GetInt32("ID"));
            
            foreach (var item in allitems) 
            {
                System.Console.WriteLine ("$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$");
                System.Console.WriteLine (item.Product.ProductName);
                System.Console.WriteLine (item.Product.InitialQuantity);
                System.Console.WriteLine (item.Product.Price);

                System.Console.WriteLine ("$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$");
            }
            

            var newOrder = _repo.CreateOrder((int)userId, charge.Amount/(float) 100);

            _context.Add (newOrder);
            _context.SaveChanges ();


            // The code below creates a record for each product in the OrderDetail table, and updates the
            // quantity for products sold.
            
            foreach (var item in allitems) 
            {
                
                var orderDetail = _repo.CreateOrderDetail(newOrder.OrderId, item.ProductId, item.Product.Price, item.Quantity, item.Product.ProductName, item.Product.Price * item.Quantity);

                _context.Add (orderDetail);

                var currentProduct = _repo.GetProductById(item.ProductId);
                 
                currentProduct.InitialQuantity -= item.Quantity;

                _context.SaveChanges ();

            }

            // The code below is for emptying the shopping cart after successful purchase.
            foreach (var item in allitems) 
            {
                _context.Items.Remove (item);
                _context.SaveChanges ();
            }

            // The code below is for sending email to the customer who checked out successfully.
            var senderEmail = new MailAddress ("demoemail536@gmail.com", "Tech Bazaar");
            var receiverEmail = new MailAddress (stripeEmail, HttpContext.Session.GetString ("username"));

            var password = "test1234%";
            var subject = "Order Confirmation";
            var body = "Thanks for ordering with us. Your products should arrive soon. We hope you give us a chance to serve you again.";

            var smtp = new SmtpClient {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential (senderEmail.Address, password),
            };

            using (var mess = new MailMessage (senderEmail, receiverEmail) {
                Subject = subject,
                    Body = body,
            }) {
                smtp.Send (mess);
            }

            return RedirectToAction ("Success");
        }

    }
}