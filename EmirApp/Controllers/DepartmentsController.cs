using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using EmirApp.DAL;
using EmirApp.Models;

namespace EmirApp.Controllers
{
    public class DepartmentsController : Controller
    {
        private UnitOfWork unitOfWork = new UnitOfWork();

        // GET: Departments
        public ActionResult Index()
        {
            var departments = unitOfWork.DepartmentRepository.Get(includeProperties: "Administrator");
                //db.Departments.Include(d => d.Administrator);
            return View(departments.ToList());
        }

        // GET: Departments/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Department department = unitOfWork.DepartmentRepository.GetByID(id);
            if (department == null)
            {
                return HttpNotFound();
            }
            return View(department);
        }

        // GET: Departments/Create
        public ActionResult Create()
        {
            PopulateDepartmentsDropDownList();
            return View();
        }
        private void PopulateDepartmentsDropDownList(object selectedDepartment = null)
        {
            var instructorsQuery = unitOfWork.InstructorRepository.Get(
             orderBy: q => q.OrderBy(i => i.FirstMidName));
            ViewBag.InstructorID = new SelectList(instructorsQuery, "ID", "FirstMidName", selectedDepartment);
        }
        // POST: Departments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "DepartmentID,Name,Budget,StartDate,InstructorID")] Department department)
        {
            if (ModelState.IsValid)
            {
                unitOfWork.DepartmentRepository.Insert(department);
                unitOfWork.Save();
                //db.Departments.Add(department);
                //db.SaveChanges();
                return RedirectToAction("Index");
            }
            var instructorsQuery = unitOfWork.InstructorRepository.Get(
             orderBy: q => q.OrderBy(i => i.FirstMidName));
            ViewBag.InstructorID = new SelectList(instructorsQuery, "ID", "FirstMidName", department.InstructorID);
            return View(department);
        }

        // GET: Departments/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Department department = unitOfWork.DepartmentRepository.GetByID(id);
            //Department department = db.Departments.Find(id);
            if (department == null)
            {
                return HttpNotFound();
            }
            var instructorsQuery = unitOfWork.InstructorRepository.Get(
             orderBy: q => q.OrderBy(i => i.FirstMidName));
            ViewBag.InstructorID = new SelectList(instructorsQuery, "ID", "FirstMidName", department.InstructorID);
            return View(department);
        }

        // POST: Departments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int? id, byte[] rowVersion)
        {
            string[] fieldsToBind = new string[] { "Name", "Budget", "StartDate", "InstructorID", "RowVersion" };

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var departmentToUpdate = unitOfWork.DepartmentRepository.GetByID(id);//db.Departments.Find(id);
            if (departmentToUpdate == null)
            {
                Department deletedDepartment = new Department();
                TryUpdateModel(deletedDepartment, fieldsToBind);
                ModelState.AddModelError(string.Empty,
                    "Unable to save changes. The department was deleted by another user.");
                var instructorsQuery1 = unitOfWork.InstructorRepository.Get(
             orderBy: q => q.OrderBy(i => i.FirstMidName));
                ViewBag.InstructorID = new SelectList(instructorsQuery1, "ID", "FirstMidName", departmentToUpdate.InstructorID);
                return View(deletedDepartment);
            }

            if (TryUpdateModel(departmentToUpdate, fieldsToBind))
            {
                try
                {
                    unitOfWork.DepartmentRepository.Update(departmentToUpdate);
                    unitOfWork.Save();
                    //db.Entry(departmentToUpdate).OriginalValues["RowVersion"] = rowVersion;
                    //db.SaveChanges();

                    return RedirectToAction("Index");
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    var entry = ex.Entries.Single();
                    var clientValues = (Department)entry.Entity;
                    var databaseEntry = entry.GetDatabaseValues();
                    if (databaseEntry == null)
                    {
                        ModelState.AddModelError(string.Empty,
                            "Unable to save changes. The department was deleted by another user.");
                    }
                    else
                    {
                        var databaseValues = (Department)databaseEntry.ToObject();

                        if (databaseValues.Name != clientValues.Name)
                            ModelState.AddModelError("Name", "Current value: "
                                + databaseValues.Name);
                        if (databaseValues.Budget != clientValues.Budget)
                            ModelState.AddModelError("Budget", "Current value: "
                                + String.Format("{0:c}", databaseValues.Budget));
                        if (databaseValues.StartDate != clientValues.StartDate)
                            ModelState.AddModelError("StartDate", "Current value: "
                                + String.Format("{0:d}", databaseValues.StartDate));
                        if (databaseValues.InstructorID != clientValues.InstructorID)
                            ModelState.AddModelError("InstructorID", "Current value: "
                                + unitOfWork.InstructorRepository.GetByID(id).FirstMidName);
                                //db.Instructors.Find(databaseValues.InstructorID).FullName);
                        ModelState.AddModelError(string.Empty, "The record you attempted to edit "
                            + "was modified by another user after you got the original value. The "
                            + "edit operation was canceled and the current values in the database "
                            + "have been displayed. If you still want to edit this record, click "
                            + "the Save button again. Otherwise click the Back to List hyperlink.");
                        departmentToUpdate.RowVersion = databaseValues.RowVersion;
                    }
                }
                catch (RetryLimitExceededException /* dex */)
                {
                    //Log the error (uncomment dex variable name and add a line here to write a log.)
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }
            var instructorsQuery = unitOfWork.InstructorRepository.Get(
            orderBy: q => q.OrderBy(i => i.FirstMidName));
            ViewBag.InstructorID = new SelectList(instructorsQuery, "ID", "FirstMidName", departmentToUpdate.InstructorID);
            //ViewBag.InstructorID = new SelectList(db.Instructors, "ID", "FullName", departmentToUpdate.InstructorID);
            return View(departmentToUpdate);
        }

        // GET: Departments/Delete/5
        public ActionResult Delete(int? id, bool? concurrencyError)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Department department = unitOfWork.DepartmentRepository.GetByID(id);
            //Department department = db.Departments.Find(id);
            if (department == null)
            {
                if (concurrencyError.GetValueOrDefault())
                {
                    return RedirectToAction("Index");
                }
                return HttpNotFound();
            }

            if (concurrencyError.GetValueOrDefault())
            {
                ViewBag.ConcurrencyErrorMessage = "The record you attempted to delete "
                    + "was modified by another user after you got the original values. "
                    + "The delete operation was canceled and the current values in the "
                    + "database have been displayed. If you still want to delete this "
                    + "record, click the Delete button again. Otherwise "
                    + "click the Back to List hyperlink.";
            }

            return View(department);
        }

        // POST: Departments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(Department department)
        {
            try
            {
                unitOfWork.DepartmentRepository.Delete(department);
                unitOfWork.Save();
                //db.Entry(department).State = EntityState.Deleted;
                //db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (DbUpdateConcurrencyException)
            {
                return RedirectToAction("Delete", new { concurrencyError = true, id = department.DepartmentID });
            }
            catch (DataException /* dex */)
            {
                //Log the error (uncomment dex variable name after DataException and add a line here to write a log.
                ModelState.AddModelError(string.Empty, "Unable to delete. Try again, and if the problem persists contact your system administrator.");
                return View(department);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                unitOfWork.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
