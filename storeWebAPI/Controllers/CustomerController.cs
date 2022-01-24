﻿using Microsoft.AspNetCore.Mvc;
using Models;
using DL;
using BL;
using CustomExceptions;
using Microsoft.Extensions.Caching.Memory;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace storeWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private IBL _bl;
        private IMemoryCache _memoryCache;
        public CustomerController(IBL bl, IMemoryCache memoryCache)
        {
            _bl = bl;
            _memoryCache = memoryCache;
        }
        // GET: api/<CustomerController>
        [HttpGet]
        public async Task<List<Customer>> GetCustomers()
        {
            List<Customer> allCus;
            if(!_memoryCache.TryGetValue("customer", out allCus)){
                allCus = await _bl.GetAllCustomersAsync();
                _memoryCache.Set("customer", allCus, new TimeSpan(0, 0, 30));
            }

            return allCus;
        }

        // GET api/<CustomerController>/5
        [HttpGet("{id}")]
        public Customer GetCustomerByID(int id)
        {
            return _bl.GetCustomerByID(id);
        }

        // POST api/<CustomerController>
        [HttpPost]
        public ActionResult PostCustomer(string _username, string _email, string _password)
        {
            try
            {
                Customer ncust = new Customer(_username, _password, _email);
                _bl.AddCustomer(_username, _email, _password);
                return Created("Successfully added", ncust);
            }
            catch(DuplicateException ex)
            {
                return Conflict(ex.Message);
            }
        }

        // PUT api/<CustomerController>/5
        [HttpPut("{id}")]
        public void PutIntoCustomer(int id, [FromBody] int value)
        {
            _bl.PutCOHIDIntoCustomer(id, value);
        }

        // DELETE api/<CustomerController>/5
        [HttpDelete("{id}")]
        public void DeleteCustomer([FromBody] Customer lostCustomer)
        {
            _bl.DeleteCustomer(lostCustomer.UserName);
        }
    }
}
