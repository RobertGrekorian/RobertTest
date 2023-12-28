using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RobertTest.Controllers.Home;
using RobertTest.Data;
using RobertTest.Models;
using RobertTest.Models.Dto;
using RobertTest.Services;
using Stripe.BillingPortal;
using System.Net;
using Stripe.Checkout;
using Microsoft.AspNetCore.Cors;
using NuGet.Protocol;
using Microsoft.AspNetCore.Authorization;
using RobertTest.Utility;


namespace RobertTest.Controllers.Customers
{
    
    public class CustomersController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _db;
        private readonly IBlobService _blobService;
        private readonly IConfiguration _config;
        
        public CustomersController(ILogger<HomeController> logger, AppDbContext db,IBlobService blobService,IConfiguration config)
        {
            _logger = logger;
            _db = db;
            _blobService = blobService;
            _config = config;
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
        public IActionResult Payment(int id)
        {
            if (id != 0)
            {
                Customer customer = _db.customers.FirstOrDefault(x => x.id == id);

                CustomerPaymentVM paymentVM = new CustomerPaymentVM();
                paymentVM.id = customer.id;
                paymentVM.name = customer.name;
                paymentVM.address = customer.address;
                paymentVM.city = customer.city;
                paymentVM.image = customer.image;
                paymentVM.payment = new Payment();
                return View(paymentVM);
            }

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(CustomerDto customerDto)
        {
            Customer customer = new Customer();
            
            string containerName = "reactstorage";
            string blobName = "";

            if (customerDto.id != 0)
            {
                customer = _db.customers.FirstOrDefault(x => x.id == customerDto.id);
                if (customerDto.image != null && customerDto.image.Length > 0)
                {
                    if (!string.IsNullOrEmpty(customer.image))
                    {
                        int x = customer.image.LastIndexOf('/');
                        bool result = await _blobService.DeleteBlob(customer.image.Substring(x), containerName);
                    }


                    blobName = Guid.NewGuid().ToString() + Path.GetExtension(customerDto.image.FileName);

                    await _blobService.UploadBlob(blobName, containerName, customerDto.image);
                    customer.image = await _blobService.GetBlob(blobName, containerName);
                }

            }

            
            customer.name = customerDto.name;
            customer.address = customerDto.address;
            customer.city = customerDto.city;

            
            if (customerDto.id == 0)
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
        [Authorize(Roles = SD.RoleAdmin)]
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
        [HttpGet]
        [Route("customers/success/{id}")]
        public async Task<ActionResult> Success(int id)
        {
            CustomerPaymentVM customerPaymentVM = new CustomerPaymentVM();
            Customer customer = _db.customers.FirstOrDefault(x => x.id == id);
            customerPaymentVM.name = customer.name;
            customerPaymentVM.address = customer.address;
            customerPaymentVM.city = customer.city;
            customerPaymentVM.image = customer.image;
            customerPaymentVM.id = id;
            if (id == 0)
            {
                return BadRequest();
            }
            else
            {
                List<Payment> customerPayments = await _db.payments.Where(x=>x.customerId == id && x.PaymentIntentId == null)
                    .ToListAsync();
                
                foreach(var payment in customerPayments) 
                {
                    var service = new Stripe.Checkout.SessionService();
                    Stripe.Checkout.Session session = service.Get(payment.SessionId);
                    if(session.PaymentStatus.ToLower() == "paid")
                    {
                        payment.PaymentIntentId = session.PaymentIntentId;
                        payment.ClientSecret = session.ClientSecret;
                        _db.payments.Update(payment);
                        _db.SaveChanges();
                        
                        
                        customerPaymentVM.payment = payment;
                    }
                }               
            }
            return View(customerPaymentVM);
        }
            [HttpPost]
        public async Task<ActionResult> Payment(CustomerPaymentVM customerPaymentVM)
        {
            ApiResponse response = new ApiResponse();
            try
            {
                Stripe.StripeConfiguration.ApiKey = _config["StripeSettings:SecretKey"];
                

                //var domain = "https://localhost:7196/";
                var domain = Request.Scheme + "://" + Request.Host.Value + "/";
                var options = new Stripe.Checkout.SessionCreateOptions
                {
                    SuccessUrl = domain+"customers/success/"+customerPaymentVM.id,
                    CancelUrl = domain + "customers/customers",
                    LineItems = new List<Stripe.Checkout.SessionLineItemOptions>(),
                    Mode = "payment",
                    
                };
                var sessionLineItem = new Stripe.Checkout.SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = "usd",
                        UnitAmount = (long ) (  customerPaymentVM.payment.Amount * 100),
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = "Payment"
                        },                       
                    },
                    Quantity = 1
                };
                options.LineItems.Add( sessionLineItem );

                var service = new Stripe.Checkout.SessionService();
                
                Stripe.Checkout.Session session = service.Create(options);

                customerPaymentVM.payment.customerId = customerPaymentVM.id;
                customerPaymentVM.payment.SessionId = session.Id;
                customerPaymentVM.payment.PaymentIntentId = session.PaymentIntentId;
                customerPaymentVM.payment.ClientSecret = session.ClientSecret;
                _db.payments.Add(customerPaymentVM.payment);
                _db.SaveChanges();
                

                string url = session.Url;
                return Redirect(url);


            }
            catch (Exception ex)
            {
                response.isSuccess = false;
                response.data = customerPaymentVM;
                response.ErrorMessage.Add(ex.Message);
                response.Status = StatusCodes.Status400BadRequest;
            }

            return BadRequest(response);

        }
    }
}
