using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using ECommerceApp.Models;
using ECommerceApp.Data;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using static Microsoft.AspNetCore.Hosting.Internal.HostingApplication;

namespace ECommerceApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly DataContext _context;

        public HomeController(DataContext context)
        {
            this._context = context;
         
        }


        // GET: /Home/
        [HttpGet]
        [Route("dashboard")]
        public IActionResult Dashboard()
        {
            if(HttpContext.Session.GetString("loggedin") == "true")
            {
                int? userId = HttpContext.Session.GetInt32("ID");
                String userName = HttpContext.Session.GetString("Name");
              
                var allproducts = _context.Products.ToList();
                float sum = 0;
                foreach(var i in allproducts)
                {
                    sum+=i.Price;
                }
                                 
                ViewBag.userName = userName;
                ViewBag.userId = userId;
                ViewBag.sum = sum;
                ViewBag.allproducts = allproducts;

                return View();
            }
            return RedirectToAction("Index", "Auth");
        }



        [HttpGet("allproducts")]
        public IActionResult AllProducts()
        {
            if(HttpContext.Session.GetString("loggedin") == "true")
            {
                var allproducts = _context.Products.ToList();
                ViewBag.allproducts = allproducts;
                float sum=0;
                foreach(var i in allproducts){
                    sum+=i.Price;
                }
                ViewBag.sum = sum;
                return View();
            }
            return RedirectToAction("Index", "Auth");
        }



        [HttpPost("createproduct")]
        public IActionResult CreateProduct(Product pro)
        {
            if(ModelState.IsValid)
            {
                Product newProduct = new Product
                {
                    ProductName = pro.ProductName,
                    ProductDescription = pro.ProductDescription,
                    InitialQuantity = pro.InitialQuantity,
                    Price = pro.Price,
                    ImageUrl=pro.ImageUrl,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                };

                _context.Add(newProduct);
                _context.SaveChanges();
                return RedirectToAction("AllProducts");
            }
            return View("AllProducts");
            
        }



        [HttpGet("orders")]
        public IActionResult OrderPage()
        {
            if(HttpContext.Session.GetString("loggedin") == "true")
            {
                var allthings = _context.Orders.Include(o=>o.Creator).ToList();
                ViewBag.allthings = allthings;
                return View();
            }
            return RedirectToAction("Index", "Auth");
        }



        // [HttpGet("SearchPage")]
        // public IActionResult SearchPage ()
        // {
        //     if(HttpContext.Session.GetString("loggedin") == "true")
        //     {
        //         System.Console.WriteLine("###############################");
        //         var allproducts = _context.Products.ToList();
        //         ViewBag.allproducts = allproducts;
        //         float sum=0;
        //         foreach(var i in allproducts){
        //             sum+=i.Price;
        //         }
        //         ViewBag.sum = sum;

        //         ViewBag.searchproduct = TempData["search"];
        //         return View("SearchPage");
        //         List<Product> searchproduct = _context.Products.Where(u=>u.ProductName.Contains(search)).ToList();
        //         System.Console.WriteLine("$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$");
        //         System.Console.WriteLine(searchproduct);
        //         System.Console.WriteLine(Request.Query["search"]);
        //     // ViewBag.searchproduct = searchproduct;
        //         TempData["search"] = searchproduct;
        //     }
        //     return RedirectToAction("Index", "Auth");
        // }

        [HttpGet("search")]
        public IActionResult Search(string search)
        {
            if(HttpContext.Session.GetString("loggedin") == "true")
            {
                // System.Console.WriteLine("###############################");
                var allproducts = _context.Products.ToList();
                ViewBag.allproducts = allproducts;
                float sum=0;
                foreach(var i in allproducts){
                    sum+=i.Price;
                }
                ViewBag.sum = sum;

                List<Product> searchproduct = _context.Products
                .Where(u=>u.ProductName.Contains(search) || u.ProductDescription.Contains(search))
                .ToList();
                System.Console.WriteLine("$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$");
                System.Console.WriteLine(searchproduct);
                System.Console.WriteLine(Request.Query["search"]);
                System.Console.WriteLine(search);
                ViewBag.searchproduct = searchproduct;
                foreach(var i in searchproduct){
                    System.Console.WriteLine(i.ProductName);
                    System.Console.WriteLine(i.ProductDescription);
                }
                return View();
            }
            return RedirectToAction("Index", "Auth");
        }
    }
}
