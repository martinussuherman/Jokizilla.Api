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
    [ODataRoutePrefix(nameof(PriceType))]
    [ApiVersionNeutral]
    public class PriceTypeController : ODataController
    {
        public PriceTypeController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [ODataRoute]
        [ProducesResponseType(typeof(ODataValue<IEnumerable<PriceTypeViewDto>>), Status200OK)]
        [EnableQuery]
        public IQueryable<PriceTypeViewDto> Get()
        {
            return _context.PriceTypes
                .ProjectTo<PriceTypeViewDto>(_mapper.ConfigurationProvider);
        }

        [ODataRoute(IdRoute)]
        [ProducesResponseType(typeof(PriceTypeViewDto), Status200OK)]
        [ProducesResponseType(Status404NotFound)]
        [EnableQuery(AllowedQueryOptions = AllowedQueryOptions.Select)]
        public SingleResult<PriceTypeViewDto> Get(byte id)
        {
            return SingleResult.Create(
                _context.PriceTypes
                    .Where(e => e.Id == id)
                    .ProjectTo<PriceTypeViewDto>(_mapper.ConfigurationProvider));
        }

        [ProducesResponseType(typeof(PriceTypeViewDto), Status201Created)]
        [ProducesResponseType(Status204NoContent)]
        [ProducesResponseType(Status400BadRequest)]
        [ProducesResponseType(Status409Conflict)]
        public async Task<IActionResult> Post([FromBody] PriceTypeUpdateDto create)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            PriceType item = _mapper.Map<PriceTypeUpdateDto, PriceType>(create);
            _context.PriceTypes.Add(item);

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

            return Created(_mapper.Map<PriceType, PriceTypeViewDto>(item));
        }

        [ODataRoute(IdRoute)]
        [ProducesResponseType(typeof(PriceTypeViewDto), Status200OK)]
        [ProducesResponseType(Status204NoContent)]
        [ProducesResponseType(Status400BadRequest)]
        [ProducesResponseType(Status404NotFound)]
        public async Task<IActionResult> Patch(byte id, [FromBody] Delta<PriceTypeUpdateDto> delta)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            PriceType item = await _context.PriceTypes.FindAsync(id);

            if (item == null)
            {
                return NotFound();
            }

            PriceTypeUpdateDto update = _mapper.Map<PriceType, PriceTypeUpdateDto>(item);
            delta.Patch(update);
            _mapper.Map(update, item);
            await _context.SaveChangesAsync();
            return Updated(_mapper.Map<PriceType, PriceTypeViewDto>(item));
        }

        [ODataRoute(IdRoute)]
        [ProducesResponseType(Status204NoContent)]
        [ProducesResponseType(Status404NotFound)]
        public async Task<IActionResult> Delete(byte id)
        {
            PriceType delete = await _context.PriceTypes.FindAsync(id);

            if (delete == null)
            {
                return NotFound();
            }

            _context.PriceTypes.Remove(delete);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private bool Exists(byte id)
        {
            return _context.PriceTypes.Any(e => e.Id == id);
        }

        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
    }
}
