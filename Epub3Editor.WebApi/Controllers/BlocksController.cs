using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Epub3Editor.Shared.Enums;
using Epub3Editor.Shared.Models;
using Epub3Editor.Shared.Requests;
using Epub3Editor.xSystem;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Epub3Editor.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlocksController : ControllerBase
    {
        private readonly Epub3System _system;

        public BlocksController(Epub3System system)
        {
            _system = system;
        }
        
        // GET: api/Blocks/5
        [HttpGet("{id}")]
        public IEnumerable<Block> Get(int id)
        {
            return _system.GetBlocks(id);
        }

        // POST: api/Blocks
        [HttpPost()]
        public int Post([FromBody] CreateBlockRequest createBlockRequest)
        {
            return _system.CreateBlock(createBlockRequest.Guid, createBlockRequest.ProjectId, createBlockRequest.Name, createBlockRequest.Content, createBlockRequest.Sort);
        }

        // // PUT: api/Projects/5
        // [HttpPut("{id}")]
        // public void Put(int id, [FromBody] Project project)
        // {
        //     _system.UpdateProject(project.Id, project.Name, project.Description, project.Css);
        // }

        // DELETE: api/Projects/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }

        [HttpPut("/api/Blocks/Up/{BlockId}")]
        public void BlockUp(int BlockId)
        {
            _system.MoveBlock(BlockId, MoveBlockEnum.Up);
        }

        [HttpPut("/api/Blocks/Down/{BlockId}")]
        public void BlockDown(int BlockId)
        {
            _system.MoveBlock(BlockId, MoveBlockEnum.Down);
        }
    }
}
