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

        // GET: api/Childrens/FromCurrentParent
        [Route("api/Childrens/FromCurrentParent")]
        public IQueryable<Childrens> GetChildrensForCurrentParent()
        {
            string userId = User.Identity.GetUserId();

            return db.Childrens.Where(idea => idea.ParentOneID == userId);
        }

        // GET: api/Childrens
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

        #region Search
        // GET: api/Childrens/Search/{keyword}
        [Route("api/Childrens/Search/{keyword}")]
        [HttpGet]
        public IQueryable<Childrens> SearchChildrens(string keyword)
        {
            return db.Childrens.Where(c => c.FirstName.Contains(keyword) ||
            c.LastName.Contains(keyword) || c.CC.Contains(keyword)
            || c.MiddleName.Contains(keyword));
        }

        // GET: api/Childrens/Search/{keyword}
        [Route("api/Childrens/Search/FromCurrentParent/{keyword}")]
        [HttpGet]
        public IQueryable<Childrens> SearchChildrensForCurrentParent(string keyword)
        {
            string userId = User.Identity.GetUserId();

            return db.Childrens.Where(c => c.FirstName.Contains(keyword) ||
            c.LastName.Contains(keyword) || c.CC.Contains(keyword)
            || c.MiddleName.Contains(keyword) && c.ParentOneID == userId);
        }
        #endregion

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