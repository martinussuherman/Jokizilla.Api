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
    [ODataRoutePrefix(nameof(ReferralSource))]
    [ApiVersionNeutral]
    public class ReferralSourceController : ODataController
    {
        public ReferralSourceController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [ODataRoute]
        [ProducesResponseType(typeof(ODataValue<IEnumerable<ReferralSourceViewDto>>), Status200OK)]
        [EnableQuery]
        public IQueryable<ReferralSourceViewDto> Get()
        {
            return _context.ReferralSources
                .AsNoTracking()
                .ProjectTo<ReferralSourceViewDto>(_mapper.ConfigurationProvider);
        }

        [ODataRoute(IdRoute)]
        [ProducesResponseType(typeof(ReferralSourceViewDto), Status200OK)]
        [ProducesResponseType(Status404NotFound)]
        [EnableQuery(AllowedQueryOptions = AllowedQueryOptions.Select)]
        public SingleResult<ReferralSourceViewDto> Get(byte id)
        {
            return SingleResult.Create(
                _context.ReferralSources
                    .AsNoTracking()
                    .Where(e => e.Id == id)
                    .ProjectTo<ReferralSourceViewDto>(_mapper.ConfigurationProvider));
        }

        [ProducesResponseType(typeof(ReferralSourceViewDto), Status201Created)]
        [ProducesResponseType(Status204NoContent)]
        [ProducesResponseType(Status400BadRequest)]
        [ProducesResponseType(Status409Conflict)]
        public async Task<IActionResult> Post([FromBody] ReferralSourceUpdateDto create)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            ReferralSource item = _mapper.Map<ReferralSourceUpdateDto, ReferralSource>(create);
            _context.ReferralSources.Add(item);

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

            return Created(_mapper.Map<ReferralSource, ReferralSourceViewDto>(item));
        }

        [ODataRoute(IdRoute)]
        [ProducesResponseType(typeof(ReferralSourceViewDto), Status200OK)]
        [ProducesResponseType(Status204NoContent)]
        [ProducesResponseType(Status400BadRequest)]
        [ProducesResponseType(Status404NotFound)]
        public async Task<IActionResult> Patch(byte id, [FromBody] Delta<ReferralSourceUpdateDto> delta)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            ReferralSource item = await _context.ReferralSources.FindAsync(id);

            if (item == null)
            {
                return NotFound();
            }

            ReferralSourceUpdateDto update = _mapper.Map<ReferralSource, ReferralSourceUpdateDto>(item);
            delta.Patch(update);
            _mapper.Map(update, item);
            await _context.SaveChangesAsync();
            return Updated(_mapper.Map<ReferralSource, ReferralSourceViewDto>(item));
        }

        [ODataRoute(IdRoute)]
        [ProducesResponseType(Status204NoContent)]
        [ProducesResponseType(Status404NotFound)]
        public async Task<IActionResult> Delete(byte id)
        {
            ReferralSource delete = await _context.ReferralSources.FindAsync(id);

            if (delete == null)
            {
                return NotFound();
            }

            _context.ReferralSources.Remove(delete);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private bool Exists(byte id)
        {
            return _context.ReferralSources.Any(e => e.Id == id);
        }

        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
    }
}
