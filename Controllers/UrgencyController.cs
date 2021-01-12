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
    [ODataRoutePrefix(nameof(Urgency))]
    [ApiVersionNeutral]
    public class UrgencyController : ODataController
    {
        public UrgencyController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [ODataRoute]
        [ProducesResponseType(typeof(ODataValue<IEnumerable<UrgencyViewDto>>), Status200OK)]
        [EnableQuery]
        public IQueryable<UrgencyViewDto> Get()
        {
            return _context.Urgencies
                .AsNoTracking()
                .ProjectTo<UrgencyViewDto>(_mapper.ConfigurationProvider);
        }

        [ODataRoute(IdRoute)]
        [ProducesResponseType(typeof(UrgencyViewDto), Status200OK)]
        [ProducesResponseType(Status404NotFound)]
        [EnableQuery(AllowedQueryOptions = AllowedQueryOptions.Select)]
        public SingleResult<UrgencyViewDto> Get(byte id)
        {
            return SingleResult.Create(
                _context.Urgencies
                    .AsNoTracking()
                    .Where(e => e.Id == id)
                    .ProjectTo<UrgencyViewDto>(_mapper.ConfigurationProvider));
        }

        [ProducesResponseType(typeof(UrgencyViewDto), Status201Created)]
        [ProducesResponseType(Status204NoContent)]
        [ProducesResponseType(Status400BadRequest)]
        [ProducesResponseType(Status409Conflict)]
        public async Task<IActionResult> Post([FromBody] UrgencyUpdateDto create)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Urgency item = _mapper.Map<UrgencyUpdateDto, Urgency>(create);
            _context.Urgencies.Add(item);

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

            return Created(_mapper.Map<Urgency, UrgencyViewDto>(item));
        }

        [ODataRoute(IdRoute)]
        [ProducesResponseType(typeof(UrgencyViewDto), Status200OK)]
        [ProducesResponseType(Status204NoContent)]
        [ProducesResponseType(Status400BadRequest)]
        [ProducesResponseType(Status404NotFound)]
        public async Task<IActionResult> Patch(byte id, [FromBody] Delta<UrgencyUpdateDto> delta)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Urgency item = await _context.Urgencies.FindAsync(id);

            if (item == null)
            {
                return NotFound();
            }

            UrgencyUpdateDto update = _mapper.Map<Urgency, UrgencyUpdateDto>(item);
            delta.Patch(update);
            _mapper.Map(update, item);
            await _context.SaveChangesAsync();
            return Updated(_mapper.Map<Urgency, UrgencyViewDto>(item));
        }

        [ODataRoute(IdRoute)]
        [ProducesResponseType(Status204NoContent)]
        [ProducesResponseType(Status404NotFound)]
        public async Task<IActionResult> Delete(byte id)
        {
            Urgency delete = await _context.Urgencies.FindAsync(id);

            if (delete == null)
            {
                return NotFound();
            }

            _context.Urgencies.Remove(delete);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private bool Exists(byte id)
        {
            return _context.Urgencies.Any(e => e.Id == id);
        }

        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
    }
}
