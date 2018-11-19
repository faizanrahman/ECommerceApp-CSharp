using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using ECommerceApp.Models;
using ECommerceApp.Data;
using System.Linq;
using Microsoft.EntityFrameworkCore;

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

                ViewBag.userName = userName;
                ViewBag.userId = userId;


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
    }
}
