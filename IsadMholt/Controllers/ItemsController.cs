﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using IsadMholt.Models;

namespace IsadMholt.Controllers
{
    public class ItemsController : Controller
    {
        private readonly ISAD251_MHoltContext _context;




        public ItemsController(ISAD251_MHoltContext context)
        {
            _context = context;
        }

        // GET: Items
        public async Task<IActionResult> Index()
        {
            return View(await _context.Items.ToListAsync());
        }

        public async Task<IActionResult> Menu()
        {
            return View(await _context.Items.ToListAsync());
        }

        // GET: Items/Details/5
        public async Task<IActionResult> addBasket(int? id, string price)
        {
            int OrderID = Convert.ToInt32(Request.Cookies["IdOrder"]);
            
            //Cheak if adding new item to basket.
            if (id == 0 || _context.ItemsOrdered.Find(OrderID, id) != null || id == null)
            {
                return View(await _context.Items.ToListAsync());
            }
            //Pull information on order and customers, to update db with new items.
            
            _context.Orders.Find(OrderID).Price = price;
            ItemsOrdered Newitem = new ItemsOrdered();
            Newitem.IdItem = Convert.ToInt32(id);
            Newitem.IdOrder = OrderID;
            Newitem.Quantity = 1;
            _context.ItemsOrdered.Add(Newitem);

            if (ModelState.IsValid)
            {
                _context.SaveChanges();
            }

            string cookieValue = Request.Cookies["O"];
            
            if (cookieValue != null)
            {
                int value = Convert.ToInt32(cookieValue);
                value += 1;
                cookieValue = value.ToString();
            }
            else
            {
                cookieValue = "1";
            }
            Response.Cookies.Append(id.ToString(), cookieValue);
            ViewBag.itemID = id;

            if (Request.Cookies["uniqueID"] == null)
            {
                return RedirectToAction("Login","Home");
            }
            return View(await _context.Items.ToListAsync());
        }

        // GET: Items/Remove/5
        public IActionResult RemoveItem(int? id)
        {
            Response.Cookies.Delete(id.ToString());

            int OrderID = Convert.ToInt32(Request.Cookies["IdOrder"]);
            _context.ItemsOrdered.Remove(_context.ItemsOrdered.Find(OrderID, id));
            if (ModelState.IsValid)
            {
                _context.SaveChanges();
            }

            return RedirectToAction("addBasket");
        }





        // GET: Items/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var items = await _context.Items
                .FirstOrDefaultAsync(m => m.IdItem == id);
            if (items == null)
            {
                return NotFound();
            }

            return View(items);
        }

        // GET: Items/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Items/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdItem,Catagory,Name,Url,Price")] Items items)
        {
            if (ModelState.IsValid)
            {
                _context.Add(items);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(items);
        }

        // GET: Items/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var items = await _context.Items.FindAsync(id);
            if (items == null)
            {
                return NotFound();
            }
            return View(items);
        }


        // POST: Items/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdItem,Catagory,Name,Url,Price")] Items items)
        {
            if (id != items.IdItem)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(items);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ItemsExists(items.IdItem))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(items);
        }

        // GET: Items/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var items = await _context.Items
                .FirstOrDefaultAsync(m => m.IdItem == id);
            if (items == null)
            {
                return NotFound();
            }

            return View(items);
        }

        // POST: Items/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var items = await _context.Items.FindAsync(id);
            _context.Items.Remove(items);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ItemsExists(int id)
        {
            return _context.Items.Any(e => e.IdItem == id);
        }

    }
}
