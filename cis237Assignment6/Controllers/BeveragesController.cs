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
            bool filterActive = false;

            int min = 0;
            int max = 3000;

            // Get data from the session.
            if (Session["name"] != null && !String.IsNullOrWhiteSpace((string)Session["name"]))
            {
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
            
            // TODO: FIX THIS ERROR
            // This never seems to be tripped despite being there, which means the filter isn't working properly.
            if (Session["active"] != null)
            {
                filterActive = (bool)Session["active"];
            }

            IEnumerable<Beverage> filtered = beveragesToSearch.Where(beverage => beverage.price >= min &&
                                                                                 beverage.price <= max &&
                                                                                 beverage.name.Contains(filterName) &&
                                                                                 beverage.pack.Contains(filterPack) &&
                                                                                 beverage.active == filterActive);

            IEnumerable<Beverage> finalFiltered = filtered.ToList();

            ViewBag.filterName = filterName;
            ViewBag.filterPack = filterPack;
            ViewBag.filterMinPrice = filterMinPrice;
            ViewBag.filterMaxPrice = filterMaxPrice;
            ViewBag.filterActive = filterActive;

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
            Boolean isActive = true;

            if(Request.Form["isActive"] == "on")
            {
                isActive = true;
            } else
            {
                isActive = false;
            }

            //Store the form data into the session so that it can be retrived later
            //on to filter the data.
            Session["name"] = name;
            Session["pack"] = pack;
            Session["min"] = min;
            Session["max"] = max;

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
