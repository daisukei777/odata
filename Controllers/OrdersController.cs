using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using odata.Models;

namespace odata.Controllers
{
    public class OrdersController : ODataController
    {
        private static OrderContext _db;

        public OrdersController(OrderContext context)
        {
            _db = context;
            if (context.Orders.Count() == 0)
            {
                context.Add(
                new Order
                {
                    Id = 2,
                    ShippingAddress = new StreetAddress { City = "New York", Street = "11 Wall Street" },
                    PartitionKey = "2"
                });

                context.SaveChanges();
            }
        }

        // You can modify here to customize query operation
        // [EnableQuery(PageSize = 3, AllowedQueryOptions = AllowedQueryOptions.Count | AllowedQueryOptions.Skip | AllowedQueryOptions.Top)]
        [EnableQuery(PageSize = 3)]
        public IActionResult Get()
        {
            return Ok(_db.Orders);
        }

        [EnableQuery]
        public IActionResult Get(int key)
        {
            return Ok(_db.Orders.FirstOrDefault(c => c.Id == key));
        }

        [EnableQuery]
        public IActionResult Post([FromBody] Order order)
        {
            _db.Orders.Add(order);
            _db.SaveChanges();
            return Created(order);
        }
    }
}