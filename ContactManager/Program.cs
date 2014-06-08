using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Description;
using System.Web.Http.Filters;
using Microsoft.Owin.Hosting;
using Owin;

namespace ContactManager
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            HttpConfiguration config = new HttpConfiguration();
            config.MapHttpAttributeRoutes();
            app.UseWebApi(config);
        }
    }

    public class Contact
    {
        public int Id { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string Email { get; set; }
    }

    public class ModelStateValidationAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext context)
        {
            if (context.ModelState.IsValid == false)
            {
                context.Response = context.Request.CreateErrorResponse(HttpStatusCode.BadRequest, context.ModelState);
            }
        }
    }

    [RoutePrefix("api/Contacts")]
    public class ContactsController : ApiController
    {
        static int _nextId = 0;
        static ConcurrentDictionary<int, Contact> _contacts = new ConcurrentDictionary<int, Contact>();

        static ContactsController()
        {
            Add(new Contact { FirstName = "Tony", LastName = "Stark", Email = "ironman@avengers.com" });
            Add(new Contact { FirstName = "Bruce", LastName = "Banner", Email = "hulk@avengers.com" });
            Add(new Contact { FirstName = "Thor", LastName = "Odinson", Email = "thor@avengers.com" });
        }

        static int GetNextId()
        {
            return Interlocked.Increment(ref _nextId);
        }

        static bool Add(Contact contact)
        {
            int id = GetNextId();
            contact.Id = id;
            return _contacts.TryAdd(id, contact);
        }

        [Route]
        public IEnumerable<Contact> Get()
        {
            return _contacts.Values.ToList();
        }

        [Route, ResponseType(typeof(Contact))]
        [ModelStateValidation]
        public IHttpActionResult Post(Contact contact)
        {
            if (Add(contact) == false)
            {
                return InternalServerError();
            }
            return Ok(contact);
        }
    }

    public class Program
    {
        static void Main(string[] args)
        {
            var url = "http://localhost:3000";
            using (WebApp.Start<Startup>(url))
            {
                Console.WriteLine("Listening on {0}...", url);
                Console.Write("Press [Enter] to quit.");
                Console.ReadLine();
            }
        }
    }
}
