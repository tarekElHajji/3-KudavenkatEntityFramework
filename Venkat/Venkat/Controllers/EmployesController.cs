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
        public ActionResult Index(string searchBy, string searchText, int? page )
        {
            IPagedList<Employe> tblEmploye;
            if (searchBy == "name")
            {
                tblEmploye = db.tblEmploye.Where(x => x.Name.StartsWith(searchText) || searchText == null)
                             .Include(e => e.tblDepartment).ToList().ToPagedList(page ?? 1 ,3);
            }
            else if (searchBy == "gender")
            {
                tblEmploye = db.tblEmploye.Where(x => x.Gender == searchText || searchText == null)
                .Include(e => e.tblDepartment).ToList().ToPagedList(page ?? 1 , 3);
            }
            else
            {
                tblEmploye = db.tblEmploye.Include(e => e.tblDepartment).ToList().ToPagedList(page ?? 1, 3);

            }
            return View(tblEmploye);
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
        [HttpPost, ActionName("Delete")]
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
    }
}
