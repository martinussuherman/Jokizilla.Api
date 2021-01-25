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
    /// Represents a RESTful service of Price Type.
    /// </summary>
    [Authorize]
    [Produces(JsonOutput)]
    [ODataRoutePrefix(nameof(PriceType))]
    [ApiVersionNeutral]
    public class PriceTypeController : ODataController
    {
        /// <summary>
        /// Price Type REST service.
        /// </summary>
        /// <param name="context">Database context.</param>
        /// <param name="mapper">AutoMapper mapping profile.</param>
        public PriceTypeController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        /// <summary>
        /// Retrieves list of Price Type.
        /// </summary>
        /// <remarks>
        /// *Anonymous Access*
        /// </remarks>
        /// <returns>List of Price Type.</returns>
        /// <response code="200">List of Price Type successfully retrieved.</response>
        [AllowAnonymous]
        [ODataRoute]
        [ProducesResponseType(typeof(ODataValue<IEnumerable<PriceTypeViewDto>>), Status200OK)]
        [EnableQuery]
        public IQueryable<PriceTypeViewDto> Get()
        {
            return _context.PriceTypes
                .AsNoTracking()
                .ProjectTo<PriceTypeViewDto>(_mapper.ConfigurationProvider);
        }

        /// <summary>
        /// Gets a single Price Type.
        /// </summary>
        /// <remarks>
        /// *Anonymous Access*
        /// </remarks>
        /// <param name="id">The requested Price Type identifier.</param>
        /// <returns>The requested Price Type.</returns>
        /// <response code="200">The Price Type was successfully retrieved.</response>
        /// <response code="404">The Price Type does not exist.</response>
        [AllowAnonymous]
        [ODataRoute(IdRoute)]
        [ProducesResponseType(typeof(PriceTypeViewDto), Status200OK)]
        [ProducesResponseType(Status404NotFound)]
        [EnableQuery(AllowedQueryOptions = AllowedQueryOptions.Select)]
        public SingleResult<PriceTypeViewDto> Get(byte id)
        {
            return SingleResult.Create(
                _context.PriceTypes
                    .AsNoTracking()
                    .Where(e => e.Id == id)
                    .ProjectTo<PriceTypeViewDto>(_mapper.ConfigurationProvider));
        }

        /// <summary>
        /// Creates a new Price Type.
        /// </summary>
        /// <remarks>
        /// *Min role: Admin*
        /// </remarks>
        /// <param name="create">The Price Type to create.</param>
        /// <returns>The created Price Type.</returns>
        /// <response code="201">The Price Type was successfully created.</response>
        /// <response code="204">The Price Type was successfully created.</response>
        /// <response code="400">The Price Type is invalid.</response>
        /// <response code="409">The Price Type with supplied id already exist.</response>
        [MultiRoleAuthorize(RoleEnum.JokizillaAdmin)]
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

        /// <summary>
        /// Updates an existing Price Type.
        /// </summary>
        /// <remarks>
        /// *Min role: Admin*
        /// </remarks>
        /// <param name="id">The requested Price Type identifier.</param>
        /// <param name="delta">The partial Price Type to update.</param>
        /// <returns>The updated Price Type.</returns>
        /// <response code="200">The Price Type was successfully updated.</response>
        /// <response code="204">The Price Type was successfully updated.</response>
        /// <response code="400">The Price Type is invalid.</response>
        /// <response code="404">The Price Type does not exist.</response>
        [MultiRoleAuthorize(RoleEnum.JokizillaAdmin)]
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

        /// <summary>
        /// Deletes a Price Type.
        /// </summary>
        /// <remarks>
        /// *Min role: Admin*
        /// </remarks>
        /// <param name="id">The Price Type to delete.</param>
        /// <returns>None</returns>
        /// <response code="204">The Price Type was successfully deleted.</response>
        /// <response code="404">The Price Type does not exist.</response>
        [MultiRoleAuthorize(RoleEnum.JokizillaAdmin)]
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
