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
    [ODataRoutePrefix(nameof(Applicant))]
    [ApiVersionNeutral]
    public class ApplicantController : ODataController
    {
        public ApplicantController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [ODataRoute]
        [ProducesResponseType(typeof(ODataValue<IEnumerable<ApplicantViewDto>>), Status200OK)]
        [EnableQuery]
        public IQueryable<ApplicantViewDto> Get()
        {
            return _context.Applicants
                .AsNoTracking()
                .ProjectTo<ApplicantViewDto>(_mapper.ConfigurationProvider);
        }

        [ODataRoute(IdRoute)]
        [ProducesResponseType(typeof(ApplicantViewDto), Status200OK)]
        [ProducesResponseType(Status404NotFound)]
        [EnableQuery(AllowedQueryOptions = AllowedQueryOptions.Select)]
        public SingleResult<ApplicantViewDto> Get(uint id)
        {
            return SingleResult.Create(
                _context.Applicants
                    .AsNoTracking()
                    .Where(e => e.Id == id)
                    .ProjectTo<ApplicantViewDto>(_mapper.ConfigurationProvider));
        }

        [ProducesResponseType(typeof(ApplicantViewDto), Status201Created)]
        [ProducesResponseType(Status204NoContent)]
        [ProducesResponseType(Status400BadRequest)]
        [ProducesResponseType(Status409Conflict)]
        public async Task<IActionResult> Post([FromBody] ApplicantUpdateDto create)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Applicant item = _mapper.Map<ApplicantUpdateDto, Applicant>(create);
            _context.Applicants.Add(item);

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

            return Created(_mapper.Map<Applicant, ApplicantViewDto>(item));
        }

        [ODataRoute(IdRoute)]
        [ProducesResponseType(typeof(ApplicantViewDto), Status200OK)]
        [ProducesResponseType(Status204NoContent)]
        [ProducesResponseType(Status400BadRequest)]
        [ProducesResponseType(Status404NotFound)]
        public async Task<IActionResult> Patch(uint id, [FromBody] Delta<ApplicantUpdateDto> delta)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Applicant item = await _context.Applicants.FindAsync(id);

            if (item == null)
            {
                return NotFound();
            }

            ApplicantUpdateDto update = _mapper.Map<Applicant, ApplicantUpdateDto>(item);
            delta.Patch(update);
            _mapper.Map(update, item);
            await _context.SaveChangesAsync();
            return Updated(_mapper.Map<Applicant, ApplicantViewDto>(item));
        }

        [ODataRoute(IdRoute)]
        [ProducesResponseType(Status204NoContent)]
        [ProducesResponseType(Status404NotFound)]
        public async Task<IActionResult> Delete(uint id)
        {
            Applicant delete = await _context.Applicants.FindAsync(id);

            if (delete == null)
            {
                return NotFound();
            }

            _context.Applicants.Remove(delete);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private bool Exists(uint id)
        {
            return _context.Applicants.Any(e => e.Id == id);
        }

        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
    }
}
