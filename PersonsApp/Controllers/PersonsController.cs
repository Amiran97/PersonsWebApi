using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PersonsApp.Models;
using PersonsApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PersonsApp.Controllers
{
    [ApiController]
    [Route("/api/v1/[controller]")]
    [Produces("application/json")]
    public class PersonsController : ControllerBase
    {
        private readonly PersonsAppDbContext context;

        public PersonsController(PersonsAppDbContext context)
        {
            this.context = context;
        }

        // GET: api/v1/persons/
        /// <summary>
        ///     Get all persons
        /// </summary>
        /// <returns>Persons list</returns>
        [HttpGet]
        [Route("")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<List<Person>> GetAll()
        {
            return Ok(context.Persons.ToList());
        }

        // GET: api/v1/persons/{id}
        /// <summary>
        ///     Get one person by Id
        /// </summary>
        /// <param name="id">use id for get person</param>
        /// <returns>one person</returns>
        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<Person> GetById(string id)
        {
            if (id == null)
                return BadRequest();
            var person = context.Persons.FirstOrDefault(p => p.Id == id);
            if (person == null)
                return NotFound();
            return Ok(person);
        }

        // POST: api/v1/persons
        [HttpPost]
        [Authorize]
        [Route("")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<Person> Create(Person person)
        {
            if(!ModelState.IsValid)
                return BadRequest();
            try
            {
                person.Id = null;
                person.ModifyDate = DateTime.Now;
                context.Persons.Add(person);
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                return BadRequest();
            }

            return Created($"/api/v1/persons/{person.Id}", person);
        }

        // PUT: api/v1/persons/{id}
        [HttpPut]
        [Route("{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Person>> Update(string id, Person person)
        {
            if (id == null || person == null || id != person.Id)
                return BadRequest();
            if (!ModelState.IsValid)
                return BadRequest();
            if (!await context.Persons.AnyAsync(p => p.Id == id))
                return NotFound();
            try {
                person.Id = id;
                person.ModifyDate = DateTime.UtcNow;
                context.Persons.Update(person);
                context.SaveChanges();
            } catch(Exception ex) {
                return BadRequest();
            }
            return Ok(person);
        }

        [HttpPatch]
        [Route("{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<Person> Patch(string id, JsonPatchDocument<Person> personPatch)
        {
            if (id == null || personPatch == null)
                return BadRequest();
            var person = context.Persons.FirstOrDefault(p => p.Id == id);
            if (person == null) 
                return NotFound();

            personPatch.ApplyTo(person);
            try
            {
                context.Persons.Update(person);
                context.SaveChanges();
            } catch (Exception ex)
            {
                return BadRequest();
            }

            return Ok(person);
        }

        // DELETE: api/v1/persons/{id}
        [HttpDelete]
        [Route("{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public ActionResult Delete(string id)
        {
            if (id == null)
                return BadRequest();
            var person = context.Persons.FirstOrDefault(p => p.Id == id);
            if (person == null)
                return NotFound();
            try
            {
                context.Persons.Remove(person);
                context.SaveChanges();
            } catch (Exception ex)
            {
                return BadRequest();
            }
            return NoContent();
        }

        [HttpHead]
        [Route("")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult Head()
        {
            return Ok();
        }

        [HttpOptions]
        [Route("")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<string> Options()
        {
            return Ok("x2 GET, POST, PUT, PATCH, DELETE, HEAD, OPTIONS");
        }
    }
}
