using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Jokizilla.Models.Models;
using Jokizilla.Models.ViewModels;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Query;
using Microsoft.AspNet.OData.Routing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static Jokizilla.Api.Misc.ApiInfo;
using static Microsoft.AspNetCore.Http.StatusCodes;

namespace Jokizilla.Api.Controllers
{
    [Produces(JsonOutput)]
    [ODataRoutePrefix(nameof(WorkLevel))]
    [ApiVersionNeutral]
    public class WorkLevelController : ODataController
    {
        public WorkLevelController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [ODataRoute]
        [ProducesResponseType(typeof(ODataValue<IEnumerable<WorkLevelViewDto>>), Status200OK)]
        [EnableQuery]
        public IQueryable<WorkLevelViewDto> Get()
        {
            return _context.WorkLevels
                .ProjectTo<WorkLevelViewDto>(_mapper.ConfigurationProvider);
        }

        [ODataRoute(IdRoute)]
        [ProducesResponseType(typeof(WorkLevelViewDto), Status200OK)]
        [ProducesResponseType(Status404NotFound)]
        [EnableQuery(AllowedQueryOptions = AllowedQueryOptions.Select)]
        public SingleResult<WorkLevelViewDto> Get(byte id)
        {
            return SingleResult.Create(
                _context.WorkLevels
                    .Where(e => e.Id == id)
                    .ProjectTo<WorkLevelViewDto>(_mapper.ConfigurationProvider));
        }

        [ProducesResponseType(typeof(WorkLevelViewDto), Status201Created)]
        [ProducesResponseType(Status204NoContent)]
        [ProducesResponseType(Status400BadRequest)]
        [ProducesResponseType(Status409Conflict)]
        public async Task<IActionResult> Post([FromBody] WorkLevelUpdateDto create)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            WorkLevel item = _mapper.Map<WorkLevelUpdateDto, WorkLevel>(create);
            _context.WorkLevels.Add(item);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (Exists(item.Id))
                {
                    return Conflict();
                }

                throw;
            }

            return Created(_mapper.Map<WorkLevel, WorkLevelViewDto>(item));
        }

        [ODataRoute(IdRoute)]
        [ProducesResponseType(typeof(WorkLevelViewDto), Status200OK)]
        [ProducesResponseType(Status204NoContent)]
        [ProducesResponseType(Status400BadRequest)]
        [ProducesResponseType(Status404NotFound)]
        public async Task<IActionResult> Patch(byte id, [FromBody] Delta<WorkLevelUpdateDto> delta)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            WorkLevel item = await _context.WorkLevels.FindAsync(id);

            if (item == null)
            {
                return NotFound();
            }

            WorkLevelUpdateDto update = _mapper.Map<WorkLevel, WorkLevelUpdateDto>(item);
            delta.Patch(update);
            _mapper.Map(update, item);
            await _context.SaveChangesAsync();
            return Updated(_mapper.Map<WorkLevel, WorkLevelViewDto>(item));
        }

        [ODataRoute(IdRoute)]
        [ProducesResponseType(Status204NoContent)]
        [ProducesResponseType(Status404NotFound)]
        public async Task<IActionResult> Delete(byte id)
        {
            WorkLevel delete = await _context.WorkLevels.FindAsync(id);

            if (delete == null)
            {
                return NotFound();
            }

            _context.WorkLevels.Remove(delete);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private bool Exists(byte id)
        {
            return _context.WorkLevels.Any(e => e.Id == id);
        }

        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
    }
}
