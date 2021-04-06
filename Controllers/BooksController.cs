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
    public class BooksController : ODataController
    {
        private BookStoreContext _db;

        public BooksController(BookStoreContext context)
        {
            _db = context;
            if (context.Books.Count() == 0)
            {
                foreach (var b in DataSource.GetBooks())
                {
                    context.Books.Add(b);
                    // context.Presses.Add(b.Press);
                }
                context.SaveChanges();
            }
        }

        [EnableQuery]
        public IActionResult Get()
        {
            return Ok(_db.Books);
        }

        [EnableQuery]
        public IActionResult Get(int key)
        {
            return Ok(_db.Books.FirstOrDefault(c => c.Id == key));
        }

        [EnableQuery]
        public IActionResult Post([FromBody] Book book)
        {
            _db.Books.Add(book);
            _db.SaveChanges();
            return Created(book);
        }
    }
}
