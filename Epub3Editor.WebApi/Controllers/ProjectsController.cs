using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Epub3Editor.Shared.Models;
using Epub3Editor.xSystem;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Epub3Editor.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectsController : ControllerBase
    {
        private readonly Epub3System _system;

        public ProjectsController(Epub3System system)
        {
            _system = system;
        }
        // GET: api/Projects
        [HttpGet]
        public IEnumerable<Project> Get()
        {
            return _system.GetProjects();
        }

        // GET: api/Projects/5
        [HttpGet("{id}", Name = "Get")]
        public Project Get(int id)
        {
            return _system.GetProject(id);
        }

        // POST: api/Projects
        [HttpPost]
        public void Post([FromBody] Project project)
        {
            _system.CreateProject(Guid.NewGuid(), project.Name, project.Description, new List<string> { "Mattias" });
        }

        // PUT: api/Projects/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] Project project)
        {
            _system.UpdateProject(project.Id, project.Name, project.Description, project.Css);
        }

        // DELETE: api/Projects/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
