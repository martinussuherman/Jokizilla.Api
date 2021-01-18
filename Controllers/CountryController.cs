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
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static Jokizilla.Api.Misc.ApiInfo;
using static Microsoft.AspNetCore.Http.StatusCodes;

namespace Jokizilla.Api.Controllers
{
    [Authorize]
    [Produces(JsonOutput)]
    [ODataRoutePrefix(nameof(Country))]
    [ApiVersionNeutral]
    public class CountryController : ODataController
    {
        public CountryController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [ODataRoute]
        [ProducesResponseType(typeof(ODataValue<IEnumerable<CountryViewDto>>), Status200OK)]
        [EnableQuery]
        public IQueryable<CountryViewDto> Get()
        {
            return _context.Countries
                .AsNoTracking()
                .ProjectTo<CountryViewDto>(_mapper.ConfigurationProvider);
        }

        [ODataRoute(IdRoute)]
        [ProducesResponseType(typeof(CountryViewDto), Status200OK)]
        [ProducesResponseType(Status404NotFound)]
        [EnableQuery(AllowedQueryOptions = AllowedQueryOptions.Select)]
        public SingleResult<CountryViewDto> Get(ushort id)
        {
            return SingleResult.Create(
                _context.Countries
                    .AsNoTracking()
                    .Where(e => e.Id == id)
                    .ProjectTo<CountryViewDto>(_mapper.ConfigurationProvider));
        }

        [ProducesResponseType(typeof(CountryViewDto), Status201Created)]
        [ProducesResponseType(Status204NoContent)]
        [ProducesResponseType(Status400BadRequest)]
        [ProducesResponseType(Status409Conflict)]
        public async Task<IActionResult> Post([FromBody] CountryUpdateDto create)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Country item = _mapper.Map<CountryUpdateDto, Country>(create);
            _context.Countries.Add(item);

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

            return Created(_mapper.Map<Country, CountryViewDto>(item));
        }

        [ODataRoute(IdRoute)]
        [ProducesResponseType(typeof(CountryViewDto), Status200OK)]
        [ProducesResponseType(Status204NoContent)]
        [ProducesResponseType(Status400BadRequest)]
        [ProducesResponseType(Status404NotFound)]
        public async Task<IActionResult> Patch(ushort id, [FromBody] Delta<CountryUpdateDto> delta)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Country item = await _context.Countries.FindAsync(id);

            if (item == null)
            {
                return NotFound();
            }

            CountryUpdateDto update = _mapper.Map<Country, CountryUpdateDto>(item);
            delta.Patch(update);
            _mapper.Map(update, item);
            await _context.SaveChangesAsync();
            return Updated(_mapper.Map<Country, CountryViewDto>(item));
        }

        [ODataRoute(IdRoute)]
        [ProducesResponseType(Status204NoContent)]
        [ProducesResponseType(Status404NotFound)]
        public async Task<IActionResult> Delete(ushort id)
        {
            Country delete = await _context.Countries.FindAsync(id);

            if (delete == null)
            {
                return NotFound();
            }

            _context.Countries.Remove(delete);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private bool Exists(ushort id)
        {
            return _context.Countries.Any(e => e.Id == id);
        }

        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
    }
}
