using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using cis237Assignment6;

namespace cis237Assignment6.Controllers
{
    [Authorize]
    public class BeveragesController : Controller
    {
        // Create the beverage connection
        private BeverageEntityConnection db = new BeverageEntityConnection();

        // List all of the beverages
        public ActionResult Index()
        {
            // Holds the entire Beverages database.
            DbSet<Beverage> beveragesToSearch = db.Beverages;

            // Hold data from the session.
            string filterName = String.Empty;
            string filterPack = String.Empty;
            string filterMinPrice = String.Empty;
            string filterMaxPrice = String.Empty;
            string filterActiveStatus = String.Empty;        
            string filterAmountToShow = String.Empty;       

            // Stores minimum and maximum values for the price.
            int min = 0;
            int max = 3000;
            int amountToShow = 50;
            bool isActive = false;


            // Get data from the session.

            // If the "name" form object in the session exists, and the text value it has is not empty...
            if (Session["name"] != null && !String.IsNullOrWhiteSpace((string)Session["name"]))
            {
                // Set the filter.
                filterName = (string)Session["name"];
            }

            if (Session["pack"] != null && !String.IsNullOrWhiteSpace((string)Session["pack"]))
            {
                filterPack = (string)Session["pack"];
            }

            if (Session["min"] != null && !String.IsNullOrWhiteSpace((string)Session["min"]))
            {
                filterMinPrice = (string)Session["min"];
                min = Int32.Parse(filterMinPrice);
            }

            if (Session["max"] != null && !String.IsNullOrWhiteSpace((string)Session["max"]))
            {
                filterMaxPrice = (string)Session["max"];
                max = Int32.Parse(filterMaxPrice);
            }
            
            // If the "active" drop down box exists in the session...
            if (Session["active"] != null)
            {
                // Set the filter's text to the "value" amount of what was in the dropdown box. 
                filterActiveStatus = (string)Session["active"];

                // "0" indicates both. Meaning that isActive won't even be used down the road.
                if(filterActiveStatus == "0")
                {

                } else if (filterActiveStatus == "1")
                {
                    // 1 = "Get beverages that are active"
                    isActive = true;
                } else if (filterActiveStatus == "2")
                {
                    // 2 = "Get beverages that are not active"
                    isActive = false;
                } else
                {
                    // This should never happen, technically. Wasn't sure what type of exception to throw...
                    throw new FormatException("Value was not 0, 1 or 2");
                }
            }

            // If there's something in the session (not first time start up)
            if (Session["atATime"] != null)
            {
                // If the form object's text is not empty
                if (!String.IsNullOrWhiteSpace((string)Session["atATime"])) {

                    // Get the value from the form object
                    filterAmountToShow = (string)Session["atATime"];

                    // Convert it to a value we can use in the query
                    amountToShow = Int32.Parse(filterAmountToShow);
                }
                // If the form object's text IS empty (but this isn't the first time we've started up)
                else {
                    // Set the amount to show to "unlimited" (at least as far as integers are concerned) - they want to show everything if the text field is blank
                    amountToShow = int.MaxValue;
                }
            }
            // If this is the first time we've started up
            else if (Session["atATime"] == null)
            {
                // Default amount for first time landing
                filterAmountToShow = "50";
            }

            // Collection
            IEnumerable<Beverage> filteredListOfBeverages;

            // Execute query based on whether or not filterActive was used.
            if (filterActiveStatus == "0")
            {
                // Don't include "isActive" filter in query.
                filteredListOfBeverages = beveragesToSearch.Where(beverage => beverage.price >= min &&
                                                                                beverage.price <= max &&
                                                                                beverage.name.Contains(filterName) &&
                                                                                beverage.pack.Contains(filterPack));
            } else {
                // Do include "isActive" in query.
                filteredListOfBeverages = beveragesToSearch.Where(beverage => beverage.price >= min &&
                                                                                 beverage.price <= max &&
                                                                                 beverage.name.Contains(filterName) &&
                                                                                 beverage.pack.Contains(filterPack) &&
                                                                                 beverage.active == isActive);
            }
            
            // Get the list of beverages from the query results.
            IEnumerable<Beverage> finalFiltered = filteredListOfBeverages.ToList();

            // Restrict the number of items to show at a time. This prevents lag in the browser. 
            // TODO: If had the time could probably implement some form of a "next page/prev page" for the list but the structure of this application isn't really supportive for it
            finalFiltered = finalFiltered.Take<Beverage>(amountToShow);

            // Put the values we just potentially retrieved from the Session's form objects into the viewbag; the Form objects will use these as values for themselves.
            ViewBag.filterName = filterName;
            ViewBag.filterPack = filterPack;
            ViewBag.filterMinPrice = filterMinPrice;
            ViewBag.filterMaxPrice = filterMaxPrice;
            ViewBag.filterActive = filterActiveStatus;
            ViewBag.filterAmountToShow = filterAmountToShow;

            return View(finalFiltered);
        }

        [HttpPost, ActionName("Filter")]
        [ValidateAntiForgeryToken]
        public ActionResult Filter()
        {
            //Get the form data that was sent out of the Request object.
            //The string that is used as a key to get the data matches the
            //name property of the form control.
            String name = Request.Form.Get("name");
            String pack = Request.Form.Get("pack");
            String min = Request.Form.Get("min");
            String max = Request.Form.Get("max");
            String active = Request.Form.Get("active");
            String atATime = Request.Form.Get("atATime");

            //Store the form data into the session so that it can be retrived later
            //on to filter the data.
            Session["name"] = name;
            Session["pack"] = pack;
            Session["min"] = min;
            Session["max"] = max;
            Session["active"] = active;
            Session["atATime"] = atATime;

            //Redirect the user to the index page. We will do the work of actually
            //fiiltering the list in the index method.
            return RedirectToAction("Index");
        }

        // GET: Beverages/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Beverage beverage = db.Beverages.Find(id);
            if (beverage == null)
            {
                return HttpNotFound();
            }
            return View(beverage);
        }

        // GET: Beverages/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Beverages/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "name,pack,price,active")] Beverage beverage)
        {
            // Generate a unique ID.
            Guid uniqueID = Guid.NewGuid();

            // Holds string representation of unique ID.
            String idString = uniqueID.ToString();

            // Apparently the db only accepts id's with a max length of 10. Cut it down.
            idString = idString.Substring(0, 10);

            // Finally, assign id.
            beverage.id = idString;

            if (ModelState.IsValid)
            {
                db.Beverages.Add(beverage);

                try
                {
                    db.SaveChanges();
                }
                catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
                {
                    Exception raise = dbEx;
                    foreach (var validationErrors in dbEx.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                            string message = string.Format("{0}:{1}",
                                validationErrors.Entry.Entity.ToString(),
                                validationError.ErrorMessage);
                            // raise a new exception nesting
                            // the current instance as InnerException
                            raise = new InvalidOperationException(message, raise);
                        }
                    }
                    throw raise;
                }

                return RedirectToAction("Index");
            }

            return View(beverage);
        }

        // GET: Beverages/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Beverage beverage = db.Beverages.Find(id);
            if (beverage == null)
            {
                return HttpNotFound();
            }
            return View(beverage);
        }

        // POST: Beverages/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,name,pack,price,active")] Beverage beverage)
        {
            if (ModelState.IsValid)
            {
                db.Entry(beverage).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(beverage);
        }

        // GET: Beverages/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Beverage beverage = db.Beverages.Find(id);
            if (beverage == null)
            {
                return HttpNotFound();
            }
            return View(beverage);
        }

        // POST: Beverages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Beverage beverage = db.Beverages.Find(id);
            db.Beverages.Remove(beverage);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
