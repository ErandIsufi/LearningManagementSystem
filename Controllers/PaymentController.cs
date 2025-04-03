using eStudentSystem.Data;
using eStudentSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class PaymentController : Controller
{
    private readonly ApplicationDbContext _context;

    public PaymentController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: Payment Index
    public async Task<IActionResult> Index(string searchString, string searchBy, bool? isPaid)
    {
        var studentsQuery = _context.Students
                                    .Include(s => s.Payments)
                                    .ThenInclude(p => p.Course)
                                    .AsQueryable();

        if (!string.IsNullOrEmpty(searchString))
        {
            if (searchBy == "StudentName")
            {
                studentsQuery = studentsQuery.Where(s => (s.FirstName + " " + s.LastName).Contains(searchString));
            }
            else if (searchBy == "Course")
            {
                studentsQuery = studentsQuery.Where(s => s.Payments.Any(p => p.Course.Title.Contains(searchString)));
            }
        }

        if (isPaid.HasValue)
        {
            studentsQuery = studentsQuery.Where(s => s.Payments.Any(p => p.IsPaid == isPaid));
        }

        var students = await studentsQuery.ToListAsync();
        return View(students);
    }


    // GET: Create Payment
    public async Task<IActionResult> CreatePayment(int? studentId)
    {
        var students = await _context.Students.ToListAsync();
        ViewData["Students"] = students;

        // Kodi leshit menxi e kum nreq smbajke robt
        List<Course> courses = new List<Course>();
        if (studentId.HasValue)
        {
            courses = await _context.Students
                .Where(s => s.Id == studentId)
                .SelectMany(s => s.Courses) // qeta duhet me qit nese ka lidhje shum me shum
                .ToListAsync();
        }
        else
        {
            courses = await _context.Courses.ToListAsync();
        }

        ViewData["Courses"] = courses;
        return View();
    }


    // POST: Create Payment
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreatePayment(Payment payment)
    {
        if (payment.Amount <= 0)
        {
            ModelState.AddModelError("Amount", "Amount must be greater than 0.");
        }

        if (ModelState.IsValid)
        {
            payment.PaymentDate = DateTime.Now;
            _context.Add(payment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // Log validation errors
        foreach (var state in ModelState)
        {
            foreach (var error in state.Value.Errors)
            {
                Console.WriteLine($"Key: {state.Key}, Error: {error.ErrorMessage}");
            }
        }

        // Reload dropdown data
        ViewData["Students"] = await _context.Students.ToListAsync();
        ViewData["Courses"] = await _context.Courses.ToListAsync();
        return View(payment);
    }

    // GET: Edit Payment
    public async Task<IActionResult> EditPayment(int id)
    {
        var payment = await _context.Payments.FirstOrDefaultAsync(p => p.PaymentId == id);

        if (payment == null)
        {
            return NotFound();
        }

        ViewData["Students"] = await _context.Students.ToListAsync();
        ViewData["Courses"] = await _context.Courses.ToListAsync();
        return View(payment);
    }

    // POST: Edit Payment
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditPayment(int id, Payment updatedPayment)
    {
        if (id != updatedPayment.PaymentId)
        {
            return BadRequest();
        }

        if (updatedPayment.Amount <= 0)
        {
            ModelState.AddModelError("Amount", "Amount must be greater than 0.");
        }

        if (ModelState.IsValid)
        {
            try
            {
                var payment = await _context.Payments.FirstOrDefaultAsync(p => p.PaymentId == id);
                if (payment == null)
                {
                    return NotFound();
                }

                payment.StudentId = updatedPayment.StudentId;
                payment.CourseId = updatedPayment.CourseId;
                payment.Amount = updatedPayment.Amount;
                payment.IsPaid = updatedPayment.IsPaid;

                _context.Update(payment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Payments.Any(p => p.PaymentId == id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        // Log validation errors
        foreach (var state in ModelState)
        {
            foreach (var error in state.Value.Errors)
            {
                Console.WriteLine($"Key: {state.Key}, Error: {error.ErrorMessage}");
            }
        }

        ViewData["Students"] = await _context.Students.ToListAsync();
        ViewData["Courses"] = await _context.Courses.ToListAsync();
        return View(updatedPayment);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ManagePayment(int paymentId, bool isPaid)
    {
        // Find the payment by ID
        var payment = await _context.Payments.FirstOrDefaultAsync(p => p.PaymentId == paymentId);

        if (payment == null)
        {
            return NotFound(); // Handle invalid paymentId
        }

        // Update the payment's status
        payment.IsPaid = isPaid;

        try
        {
            // Save changes to the database
            _context.Update(payment);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            // Handle any potential errors during database update
            ModelState.AddModelError(string.Empty, "An error occurred while updating the payment.");
        }

        // Redirect to the Index view or wherever you'd like to navigate after the update
        return RedirectToAction(nameof(Index));
    }

    // GET: Delete Payment
    public async Task<IActionResult> DeletePayment(int id)
    {
        var payment = await _context.Payments
            .Include(p => p.Student) // Optionally include related entities if needed
            .Include(p => p.Course)
            .FirstOrDefaultAsync(p => p.PaymentId == id);

        if (payment == null)
        {
            return NotFound();
        }

        return View(payment); // Pass the payment to the view for confirmation
    }
    // POST: Delete Payment
    // POST: Delete Payment
    [HttpPost, ActionName("DeletePayment")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeletePaymentConfirmed(int id)
    {
        var payment = await _context.Payments.FindAsync(id);

        if (payment == null)
        {
            return NotFound();
        }

        _context.Payments.Remove(payment);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index)); // Redirect to the index page after deletion
    }


}
