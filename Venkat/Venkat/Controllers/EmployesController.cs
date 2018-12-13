using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Venkat.Models;
using PagedList;
using PagedList.Mvc;

namespace Venkat.Controllers
{
    public class EmployesController : Controller
    {
        private EmployeeEntitie db = new EmployeeEntitie();

        // GET: Employes
        public ActionResult Index(string searchBy, string searchText, int? page, string sortBy )
        {
            ViewBag.sortByNameParameter = string.IsNullOrEmpty(sortBy) ? "name dsc": "";
            ViewBag.sortByGenderParameter = sortBy == "gender" ? "gender dsc" : "gender";
            ViewBag.sortByCityParameter = sortBy == "city" ? "city dsc" : "city";

            IQueryable<Employe> employes = db.tblEmploye;
            if (searchBy == "name")
            {
                employes = employes.Where(x => (x.Name.StartsWith(searchText)) || (searchText == null))
                             .Include(e => e.tblDepartment);
            }
            else if (searchBy == "gender")
            {
                employes = employes.Where(x => (x.Gender == searchText) || (searchText == "") || (searchText == null))
                .Include(e => e.tblDepartment);
            }
            else
            {
                employes = employes.Include(e => e.tblDepartment);

            }

            switch (sortBy)
            {
                case "name dsc": employes = employes.OrderByDescending(x => x.Name);
                    break;
                case "gender dsc": employes = employes.OrderByDescending(x => x.Gender);
                    break;
                case "gender": employes = employes.OrderBy(x => x.Gender);
                    break;
                case "city dsc":
                    employes = employes.OrderByDescending(x => x.City);
                    break;
                case "city":
                    employes = employes.OrderBy(x => x.City);
                    break;
                default: employes = employes.OrderBy(x => x.Name);
                    break;
            }
            return View(employes.ToPagedList(page ?? 1, 3));
        }

        // GET: Employes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employe employe = db.tblEmploye.Find(id);
            if (employe == null)
            {
                return HttpNotFound();
            }
            return View(employe);
        }

        // GET: Employes/Create
        public ActionResult Create()
        {
            ViewBag.DepartmentId = new SelectList(db.tblDepartment, "IdDepartment", "Name");
            return View();
        }

        // POST: Employes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IdEmployee,Name,Gender,City,DateOfBirth,DepartmentId,UrlImage,AltImage")] Employe employe)
        {
            if (ModelState.IsValid)
            {
                db.tblEmploye.Add(employe);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.DepartmentId = new SelectList(db.tblDepartment, "IdDepartment", "Name", employe.DepartmentId);
            return View(employe);
        }

        // GET: Employes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employe employe = db.tblEmploye.Find(id);
            if (employe == null)
            {
                return HttpNotFound();
            }
            ViewBag.DepartmentId = new SelectList(db.tblDepartment, "IdDepartment", "Name", employe.DepartmentId);
            return View(employe);
        }

        // POST: Employes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IdEmployee,Name,Gender,City,DateOfBirth,DepartmentId,UrlImage,AltImage")] Employe employe)
        {
           Employe employeeFromDB = db.tblEmploye.Single(emp => emp.IdEmployee == employe.IdEmployee);

            employeeFromDB.IdEmployee = employe.IdEmployee;
            employeeFromDB.Gender = employe.Gender;
            employeeFromDB.City = employe.City;
            employeeFromDB.DateOfBirth = employe.DateOfBirth;
            employeeFromDB.UrlImage = employe.UrlImage;
            employeeFromDB.AltImage = employe.AltImage;
            employeeFromDB.DepartmentId = employe.DepartmentId;

            if (ModelState.IsValid)
            {
                db.Entry(employeeFromDB).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.DepartmentId = new SelectList(db.tblDepartment, "IdDepartment", "Name", employe.DepartmentId);
            return View(employe);
        }

        // GET: Employes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employe employe = db.tblEmploye.Find(id);
            if (employe == null)
            {
                return HttpNotFound();
            }
            return View(employe);
        }

        // POST: Employes/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Employe employe = db.tblEmploye.Find(id);
            db.tblEmploye.Remove(employe);
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

        public ActionResult EmployeeByDepartment()
        {
            var employees = db.tblEmploye.Include("tblDepartment")
                                       .GroupBy(x => x.tblDepartment.Name)
                                        .Select(y => new EmployeeByDepartment
                                        {
                                            Name = y.Key,
                                            Total = y.Count()
                                        }).ToList().OrderByDescending(y => y.Total);
            return View(employees);

        }

        [HttpPost]
        public ActionResult Delete(IEnumerable<int> toDeleteEmployesIds)
        {

            foreach (var id in toDeleteEmployesIds)
            {
                var employee = db.tblEmploye.Single(e => e.IdEmployee == id);

                db.tblEmploye.Remove(employee);
            }

            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
