using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BTS.API.ENTITY;
using BTS.API.ENTITY.Authorize;
using BTS.API.SERVICE.Authorize;
using BTS.API.ENTITY.Helper;

namespace BTS.SP.API.Controllers
{
    
    public class ClientsController : Controller
    {
        private IClientService _service;
        public ClientsController(IClientService service)
        {
            _service = service;
        }
        // GET: Clients
        public async Task<ActionResult> Index()
        {
            return View(await _service.Repository.DbSet.ToListAsync());
        }

        // GET: Clients/Details/5
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Client client = await _service.Repository.DbSet.FindAsync(id);
            if (client == null)
            {
                return HttpNotFound();
            }
            return View(client);
        }

        // GET: Clients/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Clients/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Secret,Name,ApplicationType,Active,RefreshTokenLifeTime,AllowedOrigin")] Client client)
        {
            if (ModelState.IsValid)
            {
                client.Secret = HelperHash.GetHash(client.Secret);
                _service.Repository.Insert(client);
                await _service.UnitOfWork.SaveAsync();
                return RedirectToAction("Index");
            }

            return View(client);
        }

        // GET: Clients/Edit/5
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Client client = await _service.Repository.DbSet.FindAsync(id);
            if (client == null)
            {
                return HttpNotFound();
            }
            return View(client);
        }

        // POST: Clients/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Secret,Name,ApplicationType,Active,RefreshTokenLifeTime,AllowedOrigin")] Client client)
        {
            if (ModelState.IsValid)
            {
                client.Secret = HelperHash.GetHash(client.Secret);
                _service.Repository.Update(client);
                await _service.UnitOfWork.SaveAsync();
                return RedirectToAction("Index");
            }
            return View(client);
        }

        // GET: Clients/Delete/5
        public async Task<ActionResult> Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Client client = await _service.Repository.DbSet.FindAsync(id);
            if (client == null)
            {
                return HttpNotFound();
            }
            return View(client);
        }

        // POST: Clients/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            Client client = await _service.Repository.FindAsync(id);
            _service.Repository.Delete(client);
            await _service.UnitOfWork.SaveAsync();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _service.Repository.DataContext.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
