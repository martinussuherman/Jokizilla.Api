using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Jokizilla.Api.Misc;
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
    /// <summary>
    /// Represents a RESTful service of Referral Source.
    /// </summary>
    [Authorize]
    [Produces(JsonOutput)]
    [ODataRoutePrefix(nameof(ReferralSource))]
    [ApiVersionNeutral]
    public class ReferralSourceController : ODataController
    {
        /// <summary>
        /// Referral Source REST service.
        /// </summary>
        /// <param name="context">Database context.</param>
        /// <param name="mapper">AutoMapper mapping profile.</param>
        public ReferralSourceController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        /// <summary>
        /// Retrieves list of Referral Source.
        /// </summary>
        /// <remarks>
        /// *Anonymous Access*
        /// </remarks>
        /// <returns>List of Referral Source.</returns>
        /// <response code="200">List of Referral Source successfully retrieved.</response>
        [AllowAnonymous]
        [ODataRoute]
        [ProducesResponseType(typeof(ODataValue<IEnumerable<ReferralSourceViewDto>>), Status200OK)]
        [EnableQuery]
        public IQueryable<ReferralSourceViewDto> Get()
        {
            return _context.ReferralSources
                .AsNoTracking()
                .ProjectTo<ReferralSourceViewDto>(_mapper.ConfigurationProvider);
        }

        /// <summary>
        /// Gets a single Referral Source.
        /// </summary>
        /// <remarks>
        /// *Anonymous Access*
        /// </remarks>
        /// <param name="id">The requested Referral Source identifier.</param>
        /// <returns>The requested Referral Source.</returns>
        /// <response code="200">The Referral Source was successfully retrieved.</response>
        /// <response code="404">The Referral Source does not exist.</response>
        [AllowAnonymous]
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

        /// <summary>
        /// Creates a new Referral Source.
        /// </summary>
        /// <remarks>
        /// *Min role: Admin*
        /// </remarks>
        /// <param name="create">The Referral Source to create.</param>
        /// <returns>The created Referral Source.</returns>
        /// <response code="201">The Referral Source was successfully created.</response>
        /// <response code="204">The Referral Source was successfully created.</response>
        /// <response code="400">The Referral Source is invalid.</response>
        /// <response code="409">The Referral Source with supplied id already exist.</response>
        [MultiRoleAuthorize(RoleEnum.JokizillaAdmin)]
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

        /// <summary>
        /// Updates an existing Referral Source.
        /// </summary>
        /// <remarks>
        /// *Min role: Admin*
        /// </remarks>
        /// <param name="id">The requested Referral Source identifier.</param>
        /// <param name="delta">The partial Referral Source to update.</param>
        /// <returns>The updated Referral Source.</returns>
        /// <response code="200">The Referral Source was successfully updated.</response>
        /// <response code="204">The Referral Source was successfully updated.</response>
        /// <response code="400">The Referral Source is invalid.</response>
        /// <response code="404">The Referral Source does not exist.</response>
        [MultiRoleAuthorize(RoleEnum.JokizillaAdmin)]
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

        /// <summary>
        /// Deletes a Referral Source.
        /// </summary>
        /// <remarks>
        /// *Min role: Admin*
        /// </remarks>
        /// <param name="id">The Referral Source to delete.</param>
        /// <returns>None</returns>
        /// <response code="204">The Referral Source was successfully deleted.</response>
        /// <response code="404">The Referral Source does not exist.</response>
        [MultiRoleAuthorize(RoleEnum.JokizillaAdmin)]
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
