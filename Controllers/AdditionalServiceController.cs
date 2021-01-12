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
    public class AdditionalServiceController : ODataController
    {
        public AdditionalServiceController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [ODataRoute]
        [ProducesResponseType(typeof(ODataValue<IEnumerable<AdditionalServiceViewDto>>), Status200OK)]
        [EnableQuery]
        public IQueryable<AdditionalServiceViewDto> Get()
        {
            return _context.AdditionalServices
                .ProjectTo<AdditionalServiceViewDto>(_mapper.ConfigurationProvider);
        }

        [ODataRoute(IdRoute)]
        [ProducesResponseType(typeof(AdditionalServiceViewDto), Status200OK)]
        [ProducesResponseType(Status404NotFound)]
        [EnableQuery(AllowedQueryOptions = AllowedQueryOptions.Select)]
        public SingleResult<AdditionalServiceViewDto> Get(ushort id)
        {
            return SingleResult.Create(
                _context.AdditionalServices
                    .Where(e => e.Id == id)
                    .ProjectTo<AdditionalServiceViewDto>(_mapper.ConfigurationProvider));
        }

        [ProducesResponseType(typeof(AdditionalServiceViewDto), Status201Created)]
        [ProducesResponseType(Status204NoContent)]
        [ProducesResponseType(Status400BadRequest)]
        [ProducesResponseType(Status409Conflict)]
        public async Task<IActionResult> Post([FromBody] AdditionalServiceUpdateDto create)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            AdditionalService item = _mapper.Map<AdditionalServiceUpdateDto, AdditionalService>(create);
            _context.AdditionalServices.Add(item);

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

            return Created(_mapper.Map<AdditionalService, AdditionalServiceViewDto>(item));
        }

        [ODataRoute(IdRoute)]
        [ProducesResponseType(typeof(AdditionalServiceViewDto), Status200OK)]
        [ProducesResponseType(Status204NoContent)]
        [ProducesResponseType(Status400BadRequest)]
        [ProducesResponseType(Status404NotFound)]
        public async Task<IActionResult> Patch(ushort id, [FromBody] Delta<AdditionalServiceUpdateDto> delta)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            AdditionalService item = await _context.AdditionalServices.FindAsync(id);

            if (item == null)
            {
                return NotFound();
            }

            AdditionalServiceUpdateDto update = _mapper.Map<AdditionalService, AdditionalServiceUpdateDto>(item);
            delta.Patch(update);
            _mapper.Map(update, item);
            await _context.SaveChangesAsync();
            return Updated(_mapper.Map<AdditionalService, AdditionalServiceViewDto>(item));
        }

        [ODataRoute(IdRoute)]
        [ProducesResponseType(Status204NoContent)]
        [ProducesResponseType(Status404NotFound)]
        public async Task<IActionResult> Delete(ushort id)
        {
            AdditionalService delete = await _context.AdditionalServices.FindAsync(id);

            if (delete == null)
            {
                return NotFound();
            }

            _context.AdditionalServices.Remove(delete);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private bool Exists(ushort id)
        {
            return _context.AdditionalServices.Any(e => e.Id == id);
        }

        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
    }
}
