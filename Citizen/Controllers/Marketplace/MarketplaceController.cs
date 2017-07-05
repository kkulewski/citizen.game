using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Citizen.Data;
using Citizen.Models;
using Citizen.Models.MarketplaceViewModels;
using Microsoft.AspNetCore.Identity;

namespace Citizen.Controllers.Marketplace
{
    public class MarketplaceController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public MarketplaceController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Marketplace
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);

            var offers = _context.MarketplaceOffers
                .Where(offer => offer.ApplicationUser.Id == user.Id)
                .OrderBy(offer => offer.Price)
                .Include(m => m.ApplicationUser);
            
            return View("~/Views/Marketplace/Index.cshtml", await offers.ToListAsync());
        }

        // GET: Marketplace
        public async Task<IActionResult> Offers(ItemType id)
        {
            var user = _userManager.GetUserAsync(HttpContext.User);
            ViewData["ApplicationUser"] = user;

            var offers = _context.MarketplaceOffers
                .Where(offer => offer.ItemType == id)
                .OrderBy(offer => offer.Price)
                .Include(m => m.ApplicationUser);

            return View("~/Views/Marketplace/Offers.cshtml", await offers.ToListAsync());
        }

        // GET: Marketplace/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var marketplaceOffer = await _context.MarketplaceOffers
                .Include(m => m.ApplicationUser)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (marketplaceOffer == null)
            {
                return NotFound();
            }

            return View(marketplaceOffer);
        }

        // GET: Marketplace/Create
        public IActionResult Create()
        {
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Name");

            IEnumerable<ItemType> itemTypes = new List<ItemType>()
            {
                ItemType.Food,
                ItemType.Grain
            };

            ViewData["ItemType"] = new SelectList(itemTypes);

            return View();
        }

        // POST: Marketplace/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ApplicationUserId,ItemType,Amount,Price")] MarketplaceOffer marketplaceOffer)
        {
            if (ModelState.IsValid)
            {
                _context.Add(marketplaceOffer);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", marketplaceOffer.ApplicationUserId);
            return View(marketplaceOffer);
        }

        // GET: Marketplace/AddOffer
        public IActionResult AddOffer()
        {
            IEnumerable<ItemType> itemTypes = new List<ItemType>()
            {
                ItemType.Food,
                ItemType.Grain
            };

            var addMarketplaceOfferViewModel = new AddMarketplaceOfferViewModel()
            {
                ItemTypes = itemTypes
            };

            return View(addMarketplaceOfferViewModel);
        }

        //
        // POST: /Marketplace/AddOffer
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOffer(AddMarketplaceOfferViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("~/Views/Marketplace/AddOffer.cshtml", model);
            }

            var user = await _userManager.GetUserAsync(HttpContext.User);
            var offer = new MarketplaceOffer
            {
                ApplicationUserId = user.Id,
                ItemType = model.ItemType,
                Amount = model.Amount,
                Price = model.Price
            };
            user.MarketplaceOffers.Add(offer);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Marketplace/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var marketplaceOffer = await _context.MarketplaceOffers.SingleOrDefaultAsync(m => m.Id == id);
            if (marketplaceOffer == null)
            {
                return NotFound();
            }
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", marketplaceOffer.ApplicationUserId);
            return View(marketplaceOffer);
        }

        // POST: Marketplace/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ApplicationUserId,ItemType,Amount,Price")] MarketplaceOffer marketplaceOffer)
        {
            if (id != marketplaceOffer.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(marketplaceOffer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MarketplaceOfferExists(marketplaceOffer.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index");
            }
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", marketplaceOffer.ApplicationUserId);
            return View(marketplaceOffer);
        }

        // GET: Marketplace/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var marketplaceOffer = await _context.MarketplaceOffers
                .Include(m => m.ApplicationUser)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (marketplaceOffer == null)
            {
                return NotFound();
            }

            return View(marketplaceOffer);
        }

        // POST: Marketplace/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var marketplaceOffer = await _context.MarketplaceOffers.SingleOrDefaultAsync(m => m.Id == id);
            _context.MarketplaceOffers.Remove(marketplaceOffer);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool MarketplaceOfferExists(int id)
        {
            return _context.MarketplaceOffers.Any(e => e.Id == id);
        }
    }
}