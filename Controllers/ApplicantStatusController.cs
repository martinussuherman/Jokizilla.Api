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
    [ODataRoutePrefix(nameof(ApplicantStatus))]
    [ApiVersionNeutral]
    public class ApplicantStatusController : ODataController
    {
        public ApplicantStatusController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [ODataRoute]
        [ProducesResponseType(typeof(ODataValue<IEnumerable<ApplicantStatusViewDto>>), Status200OK)]
        [EnableQuery]
        public IQueryable<ApplicantStatusViewDto> Get()
        {
            return _context.ApplicantStatuses
                .AsNoTracking()
                .ProjectTo<ApplicantStatusViewDto>(_mapper.ConfigurationProvider);
        }

        [ODataRoute(IdRoute)]
        [ProducesResponseType(typeof(ApplicantStatusViewDto), Status200OK)]
        [ProducesResponseType(Status404NotFound)]
        [EnableQuery(AllowedQueryOptions = AllowedQueryOptions.Select)]
        public SingleResult<ApplicantStatusViewDto> Get(byte id)
        {
            return SingleResult.Create(
                _context.ApplicantStatuses
                    .AsNoTracking()
                    .Where(e => e.Id == id)
                    .ProjectTo<ApplicantStatusViewDto>(_mapper.ConfigurationProvider));
        }

        [ProducesResponseType(typeof(ApplicantStatusViewDto), Status201Created)]
        [ProducesResponseType(Status204NoContent)]
        [ProducesResponseType(Status400BadRequest)]
        [ProducesResponseType(Status409Conflict)]
        public async Task<IActionResult> Post([FromBody] ApplicantStatusUpdateDto create)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            ApplicantStatus item = _mapper.Map<ApplicantStatusUpdateDto, ApplicantStatus>(create);
            _context.ApplicantStatuses.Add(item);

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

            return Created(_mapper.Map<ApplicantStatus, ApplicantStatusViewDto>(item));
        }

        [ODataRoute(IdRoute)]
        [ProducesResponseType(typeof(ApplicantStatusViewDto), Status200OK)]
        [ProducesResponseType(Status204NoContent)]
        [ProducesResponseType(Status400BadRequest)]
        [ProducesResponseType(Status404NotFound)]
        public async Task<IActionResult> Patch(byte id, [FromBody] Delta<ApplicantStatusUpdateDto> delta)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            ApplicantStatus item = await _context.ApplicantStatuses.FindAsync(id);

            if (item == null)
            {
                return NotFound();
            }

            ApplicantStatusUpdateDto update = _mapper.Map<ApplicantStatus, ApplicantStatusUpdateDto>(item);
            delta.Patch(update);
            _mapper.Map(update, item);
            await _context.SaveChangesAsync();
            return Updated(_mapper.Map<ApplicantStatus, ApplicantStatusViewDto>(item));
        }

        [ODataRoute(IdRoute)]
        [ProducesResponseType(Status204NoContent)]
        [ProducesResponseType(Status404NotFound)]
        public async Task<IActionResult> Delete(byte id)
        {
            ApplicantStatus delete = await _context.ApplicantStatuses.FindAsync(id);

            if (delete == null)
            {
                return NotFound();
            }

            _context.ApplicantStatuses.Remove(delete);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private bool Exists(byte id)
        {
            return _context.ApplicantStatuses.Any(e => e.Id == id);
        }

        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
    }
}
