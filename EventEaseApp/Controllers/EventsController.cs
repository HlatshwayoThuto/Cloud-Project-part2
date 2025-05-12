using EventEaseApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EventEaseApp.Controllers
{
    public class EventsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EventsController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var events = await _context.Events.Include(b => b.Venue).ToListAsync();
            return View(events);
        }

        public IActionResult Create()
        {
            //ViewData["VenueID"] = new SelectList(_context.Venue, "VenueID", "VenueName");
            ViewBag.VenueID = new SelectList(_context.Venue, "VenueID", "Locations");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Events events)
        {
            if (ModelState.IsValid)
            {
                _context.Add(events);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Event ctreated successfully.";
                return RedirectToAction(nameof(Index));

            }
            ViewBag.VenueID = new SelectList(_context.Venue, "VenueID", "Locations", events.VenueID);
            return View(events);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var events = await _context.Events.Include(e => e.Venue)
            .FirstOrDefaultAsync(e => e.EventID == id);

            if (events == null)
            {

                return NotFound();

            }
            return View(events);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var events = await _context.Events.Include(m => m.Venue).FirstOrDefaultAsync(m => m.EventID == id);

            if (events == null)
            {

                return NotFound();

            }
            return View(events);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
           
                var events = await _context.Events.FindAsync(id);
            
            if (events == null) return NotFound();

            var isBooked = await _context.Booking.AnyAsync(b => b.EventID == id);
            if (isBooked)
            {
                TempData["ErrorMessage"] = "Cannot delete event because it has an existing booking.";
                return RedirectToAction(nameof(Index));
            }
                _context.Remove(events);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Event deleted successfully.";
                return RedirectToAction(nameof(Index));
            }

        private bool EventsExists(int id)
        {
            return _context.Events.Any(e => e.EventID == id);
        }
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var events = await _context.Events.FindAsync(id);
            if (events == null)
            {
                return NotFound();
            }
            // ✅ Repopulate dropdown
            ViewBag.VenueID = new SelectList(_context.Venue, "VenueID", "Locations", events.VenueID);
            return View(events);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Events events)
        {
            if (id != events.EventID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {    
                _context.Update(events);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Event updated successfully.";
                return RedirectToAction(nameof(Index));
             
            }
            // ✅ Repopulate dropdown again in case of error
            ViewBag.VenueID = new SelectList(_context.Venue, "VenueID", "Locations", events.VenueID);
            return View(events);
        }
    }
}

