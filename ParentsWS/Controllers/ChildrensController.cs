using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using ParentsWS.Models;
using Microsoft.AspNet.Identity;

namespace ParentsWS.Controllers
{
    [Authorize]
    public class ChildrensController : ApiController
    {
        private ChildrensContext db = new ChildrensContext();

        // GET: api/Childrens/ForCurrentParent
        [Route("api/Childrens/ForCurrentParent")]
        public IQueryable<Childrens> GetChildrensForCurrentParent()
        {
            string userId = User.Identity.GetUserId();

            return db.Childrens.Where(idea => idea.ParentOneID == userId);
        }

        public IQueryable<Childrens> GetChildrens()
        {
            return db.Childrens;
        }

        // GET: api/Childrens/5
        [ResponseType(typeof(Childrens))]
        public IHttpActionResult GetChildrens(int id)
        {
            Childrens childrens = db.Childrens.Find(id);
            if (childrens == null)
            {
                return NotFound();
            }

            return Ok(childrens);
        }

        // PUT: api/Childrens/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutChildrens(int id, Childrens childrens)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != childrens.Id)
            {
                return BadRequest();
            }

            var userId = User.Identity.GetUserId();

            if(userId != childrens.ParentOneID)
            {
                return StatusCode(HttpStatusCode.Conflict);
            }

            db.Entry(childrens).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ChildrensExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Childrens
        [ResponseType(typeof(Childrens))]
        public IHttpActionResult PostChildrens(Childrens childrens)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            childrens.ParentOneID = User.Identity.GetUserId();

            db.Childrens.Add(childrens);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = childrens.Id }, childrens);
        }

        // DELETE: api/Childrens/5
        [ResponseType(typeof(Childrens))]
        public IHttpActionResult DeleteChildrens(int id)
        {
            Childrens childrens = db.Childrens.Find(id);
            if (childrens == null)
            {
                return NotFound();
            }

            string userId = User.Identity.GetUserId();

            if(userId != childrens.ParentOneID)
            {
                return StatusCode(HttpStatusCode.Conflict);
            }

            db.Childrens.Remove(childrens);
            db.SaveChanges();

            return Ok(childrens);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ChildrensExists(int id)
        {
            return db.Childrens.Count(e => e.Id == id) > 0;
        }
    }
}