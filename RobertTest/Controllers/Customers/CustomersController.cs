using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RobertTest.Controllers.Home;
using RobertTest.Data;
using RobertTest.Models;

namespace RobertTest.Controllers.Customers
{
    public class CustomersController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _db;

        public CustomersController(ILogger<HomeController> logger, AppDbContext db)
        {
            _logger = logger;
            _db = db;
        }
        public IActionResult Customers()
        {
            IEnumerable<Customer> customers = _db.customers.ToList();

            return View(customers);
        }
        public IActionResult Create(int id) 
        {
            if(id != 0)
            {
                Customer customer = _db.customers.FirstOrDefault(x => x.id == id);
                return View(customer);
            }

           return View();
        }
        [HttpPost]
        public IActionResult Create(Customer customer)
        {
            if (customer.id == 0)
            {
                _db.customers.Add(customer);
            }
            else
            {
                _db.customers.Update(customer);
            }
            _db.SaveChanges();
            return RedirectToAction("customers");
        }

        [HttpGet]
        [Route("customers/getallcustomers")]

        public async Task<ActionResult> GetAllCustomers()
        {
            IEnumerable<Customer> customers = await _db.customers.ToListAsync();
            ApiResponse response = new ApiResponse();

            response.isSuccess = true;
            response.data = customers;
            response.Status = StatusCodes.Status200OK;


            return Ok(response);

        }

        [HttpDelete]
        public async Task<ActionResult> DeleteCustomer(int id)
        {
            ApiResponse response = new ApiResponse();
            try
            {

                Customer customer = await _db.customers.FindAsync(id);
                _db.customers.Remove(customer);
                _db.SaveChanges();


                response.isSuccess = true;
                response.data = id;
                response.Status = StatusCodes.Status200OK;
                return RedirectToAction("customers","customers");
            }
            catch (Exception ex)
            {
                response.isSuccess = false;
                response.data = id;
                response.ErrorMessage.Add(ex.Message);
                response.Status = StatusCodes.Status400BadRequest;
            }

            return BadRequest(response);

        }
    }
}
