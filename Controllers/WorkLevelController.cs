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
    /// Represents a RESTful service of Work Level.
    /// </summary>
    [Authorize]
    [Produces(JsonOutput)]
    [ODataRoutePrefix(nameof(WorkLevel))]
    [ApiVersionNeutral]
    public class WorkLevelController : ODataController
    {
        /// <summary>
        /// Work Level REST service.
        /// </summary>
        /// <param name="context">Database context.</param>
        /// <param name="mapper">AutoMapper mapping profile.</param>
        public WorkLevelController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        /// <summary>
        /// Retrieves list of Work Level.
        /// </summary>
        /// <remarks>
        /// *Anonymous Access*
        /// </remarks>
        /// <returns>List of Work Level.</returns>
        /// <response code="200">List of Work Level successfully retrieved.</response>
        [AllowAnonymous]
        [ODataRoute]
        [ProducesResponseType(typeof(ODataValue<IEnumerable<WorkLevelViewDto>>), Status200OK)]
        [EnableQuery]
        public IQueryable<WorkLevelViewDto> Get()
        {
            return _context.WorkLevels
                .AsNoTracking()
                .ProjectTo<WorkLevelViewDto>(_mapper.ConfigurationProvider);
        }

        /// <summary>
        /// Gets a single Work Level.
        /// </summary>
        /// <remarks>
        /// *Anonymous Access*
        /// </remarks>
        /// <param name="id">The requested Work Level identifier.</param>
        /// <returns>The requested Work Level.</returns>
        /// <response code="200">The Work Level was successfully retrieved.</response>
        /// <response code="404">The Work Level does not exist.</response>
        [AllowAnonymous]
        [ODataRoute(IdRoute)]
        [ProducesResponseType(typeof(WorkLevelViewDto), Status200OK)]
        [ProducesResponseType(Status404NotFound)]
        [EnableQuery(AllowedQueryOptions = AllowedQueryOptions.Select)]
        public SingleResult<WorkLevelViewDto> Get(byte id)
        {
            return SingleResult.Create(
                _context.WorkLevels
                    .AsNoTracking()
                    .Where(e => e.Id == id)
                    .ProjectTo<WorkLevelViewDto>(_mapper.ConfigurationProvider));
        }

        /// <summary>
        /// Creates a new Work Level.
        /// </summary>
        /// <remarks>
        /// *Min role: Admin*
        /// </remarks>
        /// <param name="create">The Work Level to create.</param>
        /// <returns>The created Work Level.</returns>
        /// <response code="201">The Work Level was successfully created.</response>
        /// <response code="204">The Work Level was successfully created.</response>
        /// <response code="400">The Work Level is invalid.</response>
        /// <response code="409">The Work Level with supplied id already exist.</response>
        [MultiRoleAuthorize(RoleEnum.JokizillaAdmin)]
        [ProducesResponseType(typeof(WorkLevelViewDto), Status201Created)]
        [ProducesResponseType(Status204NoContent)]
        [ProducesResponseType(Status400BadRequest)]
        [ProducesResponseType(Status409Conflict)]
        public async Task<IActionResult> Post([FromBody] WorkLevelUpdateDto create)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            WorkLevel item = _mapper.Map<WorkLevelUpdateDto, WorkLevel>(create);
            _context.WorkLevels.Add(item);

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

            return Created(_mapper.Map<WorkLevel, WorkLevelViewDto>(item));
        }

        /// <summary>
        /// Updates an existing Work Level.
        /// </summary>
        /// <remarks>
        /// *Min role: Admin*
        /// </remarks>
        /// <param name="id">The requested Work Level identifier.</param>
        /// <param name="delta">The partial Work Level to update.</param>
        /// <returns>The updated Work Level.</returns>
        /// <response code="200">The Work Level was successfully updated.</response>
        /// <response code="204">The Work Level was successfully updated.</response>
        /// <response code="400">The Work Level is invalid.</response>
        /// <response code="404">The Work Level does not exist.</response>
        [MultiRoleAuthorize(RoleEnum.JokizillaAdmin)]
        [ODataRoute(IdRoute)]
        [ProducesResponseType(typeof(WorkLevelViewDto), Status200OK)]
        [ProducesResponseType(Status204NoContent)]
        [ProducesResponseType(Status400BadRequest)]
        [ProducesResponseType(Status404NotFound)]
        public async Task<IActionResult> Patch(byte id, [FromBody] Delta<WorkLevelUpdateDto> delta)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            WorkLevel item = await _context.WorkLevels.FindAsync(id);

            if (item == null)
            {
                return NotFound();
            }

            WorkLevelUpdateDto update = _mapper.Map<WorkLevel, WorkLevelUpdateDto>(item);
            delta.Patch(update);
            _mapper.Map(update, item);
            await _context.SaveChangesAsync();
            return Updated(_mapper.Map<WorkLevel, WorkLevelViewDto>(item));
        }

        /// <summary>
        /// Deletes a Work Level.
        /// </summary>
        /// <remarks>
        /// *Min role: Admin*
        /// </remarks>
        /// <param name="id">The Work Level to delete.</param>
        /// <returns>None</returns>
        /// <response code="204">The Work Level was successfully deleted.</response>
        /// <response code="404">The Work Level does not exist.</response>
        [MultiRoleAuthorize(RoleEnum.JokizillaAdmin)]
        [ODataRoute(IdRoute)]
        [ProducesResponseType(Status204NoContent)]
        [ProducesResponseType(Status404NotFound)]
        public async Task<IActionResult> Delete(byte id)
        {
            WorkLevel delete = await _context.WorkLevels.FindAsync(id);

            if (delete == null)
            {
                return NotFound();
            }

            _context.WorkLevels.Remove(delete);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private bool Exists(byte id)
        {
            return _context.WorkLevels.Any(e => e.Id == id);
        }

        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
    }
}
