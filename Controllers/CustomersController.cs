using AcxiomCRM.Data;
using AcxiomCRM.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AcxiomCRM.Controllers
{
    public class CustomersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CustomersController> _logger;

        public CustomersController(ApplicationDbContext context, ILogger<CustomersController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Customers
        // Supports searching by code, name, email, phone, company, city, state, or status
        public async Task<IActionResult> Index(string? searchString)
        {
            ViewData["CurrentFilter"] = searchString;

            var customersQuery = _context.Customers.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                var term = searchString.Trim();
                var lowerTerm = term.ToLower();

                customersQuery = customersQuery.Where(c =>
                    c.CustomerCode.ToLower().Contains(lowerTerm) ||
                    c.CustomerName.ToLower().Contains(lowerTerm) ||
                    c.Email.ToLower().Contains(lowerTerm) ||
                    c.Phone.Contains(term) ||
                    (c.CompanyName != null && c.CompanyName.ToLower().Contains(lowerTerm)) ||
                    c.City.ToLower().Contains(lowerTerm) ||
                    c.State.ToLower().Contains(lowerTerm) ||
                    c.Status.ToLower().Contains(lowerTerm));
            }

            var customers = await customersQuery
                .OrderByDescending(c => c.CreatedDate)
                .ToListAsync();

            return View(customers);
        }

        // GET: Customers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.CustomerId == id);

            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // GET: Customers/Create
        public async Task<IActionResult> Create()
        {
            var model = new Customer
            {
                CustomerCode = await GenerateCustomerCodeAsync(),
                Status = "Active"
            };
            return View(model);
        }

        // POST: Customers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CustomerId,CustomerCode,CustomerName,Email,Phone,CompanyName,Address,City,State,Status")] Customer customer)
        {
            // Auto-generate code if empty
            if (string.IsNullOrWhiteSpace(customer.CustomerCode))
            {
                customer.CustomerCode = await GenerateCustomerCodeAsync();
                ModelState.Remove(nameof(Customer.CustomerCode));
            }

            // Normalization
            customer.CustomerCode = customer.CustomerCode.Trim().ToUpperInvariant();
            customer.CustomerName = customer.CustomerName?.Trim() ?? string.Empty;
            customer.Email = customer.Email?.Trim() ?? string.Empty;
            customer.Phone = customer.Phone?.Trim() ?? string.Empty;
            customer.CompanyName = string.IsNullOrWhiteSpace(customer.CompanyName) ? null : customer.CompanyName.Trim();
            customer.Address = customer.Address?.Trim() ?? string.Empty;
            customer.City = customer.City?.Trim() ?? string.Empty;
            customer.State = customer.State?.Trim() ?? string.Empty;
            customer.Status = string.IsNullOrWhiteSpace(customer.Status) ? "Active" : customer.Status.Trim();

            // Set audit properties on the server
            customer.CreatedDate = DateTime.UtcNow;
            customer.CreatedBy = (User?.Identity?.IsAuthenticated == true && !string.IsNullOrWhiteSpace(User.Identity.Name))
                ? User.Identity.Name
                : "System";

            var normalizedEmail = customer.Email.ToLowerInvariant();
            var normalizedPhone = customer.Phone;
            var normalizedCode = customer.CustomerCode;

            // Check for duplicate CustomerCode
            if (await _context.Customers.AnyAsync(c => c.CustomerCode == normalizedCode))
            {
                ModelState.AddModelError(nameof(Customer.CustomerCode), "A customer with this Customer Code already exists.");
            }

            // Check for duplicate Email
            if (await _context.Customers.AnyAsync(c => c.Email.ToLower() == normalizedEmail))
            {
                ModelState.AddModelError(nameof(Customer.Email), "A customer with this email address already exists.");
            }

            // Check for duplicate Phone
            if (await _context.Customers.AnyAsync(c => c.Phone == normalizedPhone))
            {
                ModelState.AddModelError(nameof(Customer.Phone), "A customer with this phone number already exists.");
            }

            if (!ModelState.IsValid)
            {
                return View(customer);
            }

            try
            {
                _context.Add(customer);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Customer '{customer.CustomerName}' ({customer.CustomerCode}) created successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database update error while creating customer.");
                ModelState.AddModelError(string.Empty, "Unable to save changes. A customer with the same Customer Code, Email, or Phone may already exist.");
                return View(customer);
            }
        }

        // GET: Customers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // POST: Customers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CustomerId,CustomerCode,CustomerName,Email,Phone,CompanyName,Address,City,State,Status")] Customer customer)
        {
            if (id != customer.CustomerId)
            {
                return BadRequest();
            }

            var existingCustomer = await _context.Customers
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.CustomerId == id);

            if (existingCustomer == null)
            {
                return NotFound();
            }

            // Preserve creation audit fields
            customer.CreatedDate = existingCustomer.CreatedDate;
            customer.CreatedBy = existingCustomer.CreatedBy;

            // Normalization
            customer.CustomerCode = customer.CustomerCode?.Trim().ToUpperInvariant() ?? string.Empty;
            customer.CustomerName = customer.CustomerName?.Trim() ?? string.Empty;
            customer.Email = customer.Email?.Trim() ?? string.Empty;
            customer.Phone = customer.Phone?.Trim() ?? string.Empty;
            customer.CompanyName = string.IsNullOrWhiteSpace(customer.CompanyName) ? null : customer.CompanyName.Trim();
            customer.Address = customer.Address?.Trim() ?? string.Empty;
            customer.City = customer.City?.Trim() ?? string.Empty;
            customer.State = customer.State?.Trim() ?? string.Empty;
            customer.Status = string.IsNullOrWhiteSpace(customer.Status) ? "Active" : customer.Status.Trim();

            var normalizedEmail = customer.Email.ToLowerInvariant();
            var normalizedPhone = customer.Phone;
            var normalizedCode = customer.CustomerCode;

            // Duplicate checks excluding this customer
            if (await _context.Customers.AnyAsync(c => c.CustomerId != customer.CustomerId && c.CustomerCode == normalizedCode))
            {
                ModelState.AddModelError(nameof(Customer.CustomerCode), "Another customer with this Customer Code already exists.");
            }

            if (await _context.Customers.AnyAsync(c => c.CustomerId != customer.CustomerId && c.Email.ToLower() == normalizedEmail))
            {
                ModelState.AddModelError(nameof(Customer.Email), "Another customer with this email address already exists.");
            }

            if (await _context.Customers.AnyAsync(c => c.CustomerId != customer.CustomerId && c.Phone == normalizedPhone))
            {
                ModelState.AddModelError(nameof(Customer.Phone), "Another customer with this phone number already exists.");
            }

            if (!ModelState.IsValid)
            {
                return View(customer);
            }

            try
            {
                _context.Update(customer);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Customer '{customer.CustomerName}' ({customer.CustomerCode}) updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await CustomerExists(customer.CustomerId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database update error while editing customer {CustomerId}.", customer.CustomerId);
                ModelState.AddModelError(string.Empty, "Unable to save changes. A customer with the same code, email, or phone may conflict with another record.");
                return View(customer);
            }
        }

        // GET: Customers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.CustomerId == id);

            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // POST: Customers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer != null)
            {
                var customerInfo = $"{customer.CustomerName} ({customer.CustomerCode})";
                _context.Customers.Remove(customer);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Customer '{customerInfo}' was successfully deleted.";
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> CustomerExists(int id)
        {
            return await _context.Customers.AnyAsync(e => e.CustomerId == id);
        }

        private async Task<string> GenerateCustomerCodeAsync()
        {
            var lastId = await _context.Customers
                .OrderByDescending(c => c.CustomerId)
                .Select(c => c.CustomerId)
                .FirstOrDefaultAsync();

            var nextNumber = lastId + 1;
            var code = $"CUST-{nextNumber:D5}";

            while (await _context.Customers.AnyAsync(c => c.CustomerCode == code))
            {
                nextNumber++;
                code = $"CUST-{nextNumber:D5}";
            }

            return code;
        }
    }
}
