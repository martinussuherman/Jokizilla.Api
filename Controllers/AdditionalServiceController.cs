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
    /// Represents a RESTful service of Additional Service.
    /// </summary>
    [Authorize]
    [Produces(JsonOutput)]
    [ODataRoutePrefix(nameof(AdditionalService))]
    [ApiVersionNeutral]
    public class AdditionalServiceController : ODataController
    {
        /// <summary>
        /// Additional Service REST service.
        /// </summary>
        /// <param name="context">Database context.</param>
        /// <param name="mapper">AutoMapper mapping profile.</param>
        public AdditionalServiceController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        /// <summary>
        /// Retrieves list of Additional Service.
        /// </summary>
        /// <remarks>
        /// *Anonymous Access*
        /// </remarks>
        /// <returns>List of Additional Service.</returns>
        /// <response code="200">List of Additional Service successfully retrieved.</response>
        [AllowAnonymous]
        [ODataRoute]
        [ProducesResponseType(typeof(ODataValue<IEnumerable<AdditionalServiceViewDto>>), Status200OK)]
        [EnableQuery]
        public IQueryable<AdditionalServiceViewDto> Get()
        {
            return _context.AdditionalServices
                .AsNoTracking()
                .ProjectTo<AdditionalServiceViewDto>(_mapper.ConfigurationProvider);
        }

        /// <summary>
        /// Gets a single Additional Service.
        /// </summary>
        /// <remarks>
        /// *Anonymous Access*
        /// </remarks>
        /// <param name="id">The requested Additional Service identifier.</param>
        /// <returns>The requested Additional Service.</returns>
        /// <response code="200">The Additional Service was successfully retrieved.</response>
        /// <response code="404">The Additional Service does not exist.</response>
        [AllowAnonymous]
        [ODataRoute(IdRoute)]
        [ProducesResponseType(typeof(AdditionalServiceViewDto), Status200OK)]
        [ProducesResponseType(Status404NotFound)]
        [EnableQuery(AllowedQueryOptions = AllowedQueryOptions.Select)]
        public SingleResult<AdditionalServiceViewDto> Get(ushort id)
        {
            return SingleResult.Create(
                _context.AdditionalServices
                    .AsNoTracking()
                    .Where(e => e.Id == id)
                    .ProjectTo<AdditionalServiceViewDto>(_mapper.ConfigurationProvider));
        }

        /// <summary>
        /// Creates a new Additional Service.
        /// </summary>
        /// <remarks>
        /// *Min role: Admin*
        /// </remarks>
        /// <param name="create">The Additional Service to create.</param>
        /// <returns>The created Additional Service.</returns>
        /// <response code="201">The Additional Service was successfully created.</response>
        /// <response code="204">The Additional Service was successfully created.</response>
        /// <response code="400">The Additional Service is invalid.</response>
        /// <response code="409">The Additional Service with supplied id already exist.</response>
        [MultiRoleAuthorize(RoleEnum.JokizillaAdmin)]
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

        /// <summary>
        /// Updates an existing Additional Service.
        /// </summary>
        /// <remarks>
        /// *Min role: Admin*
        /// </remarks>
        /// <param name="id">The requested Additional Service identifier.</param>
        /// <param name="delta">The partial Additional Service to update.</param>
        /// <returns>The updated Additional Service.</returns>
        /// <response code="200">The Additional Service was successfully updated.</response>
        /// <response code="204">The Additional Service was successfully updated.</response>
        /// <response code="400">The Additional Service is invalid.</response>
        /// <response code="404">The Additional Service does not exist.</response>
        [MultiRoleAuthorize(RoleEnum.JokizillaAdmin)]
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

        /// <summary>
        /// Deletes a Additional Service.
        /// </summary>
        /// <remarks>
        /// *Min role: Admin*
        /// </remarks>
        /// <param name="id">The Additional Service to delete.</param>
        /// <returns>None</returns>
        /// <response code="204">The Additional Service was successfully deleted.</response>
        /// <response code="404">The Additional Service does not exist.</response>
        [MultiRoleAuthorize(RoleEnum.JokizillaAdmin)]
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
