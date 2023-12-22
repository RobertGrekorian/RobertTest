using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RobertTest.Controllers.Home;
using RobertTest.Data;
using RobertTest.Models;
using RobertTest.Models.Dto;
using RobertTest.Services;

namespace RobertTest.Controllers.Customers
{
    public class CustomersController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _db;
        private readonly IBlobService _blobService;

        public CustomersController(ILogger<HomeController> logger, AppDbContext db,IBlobService blobService)
        {
            _logger = logger;
            _db = db;
            _blobService = blobService;
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
        public async Task<IActionResult> Create(CustomerDto customerDto)
        {
            Customer customer = new Customer();
            
            string containerName = "reactstorage";
            string blobName = "";

            if (customerDto.image != null  && customerDto.image.Length > 0)
            {
                if (customerDto.id != 0)
                {
                    customer = _db.customers.FirstOrDefault(x => x.id == customerDto.id);
                    if (!string.IsNullOrEmpty(customer.image))
                    {
                        int x = customer.image.LastIndexOf('/');
                        bool result = await _blobService.DeleteBlob(customer.image.Substring(x), containerName);
                    }
                }

                blobName = Guid.NewGuid().ToString() + Path.GetExtension(customerDto.image.FileName);

                await _blobService.UploadBlob(blobName, containerName, customerDto.image);
                customer.image = await  _blobService.GetBlob(blobName, containerName);

            }

            
            customer.name = customerDto.name;
            customer.address = customerDto.address;
            customer.city = customerDto.city;

            
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
                string containerName = "reactstorage";
                string blobName = "";
                Customer customer = await _db.customers.FindAsync(id);
                if (customer.image != null)
                {
                    int x = customer.image.LastIndexOf('/');
                    bool result = await _blobService.DeleteBlob(customer.image.Substring(x), containerName);
                }
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
