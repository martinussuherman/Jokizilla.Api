using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Jokizilla.Api.Misc;
using Jokizilla.Models.Models;
using Jokizilla.Models.ViewModels;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Routing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static Jokizilla.Api.Misc.ApiInfo;
using static Microsoft.AspNetCore.Http.StatusCodes;

namespace Jokizilla.Api.Controllers
{
    /// <summary>
    /// Represents a RESTful service of Service.
    /// </summary>
    [Authorize]
    [Produces(JsonOutput)]
    [ODataRoutePrefix(nameof(Service))]
    [ApiVersionNeutral]
    public class ServiceController : ODataController
    {
        /// <summary>
        /// Service REST service.
        /// </summary>
        /// <param name="context">Database context.</param>
        /// <param name="mapper">AutoMapper mapping profile.</param>
        public ServiceController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        /// <summary>
        /// Retrieves list of Service.
        /// </summary>
        /// <remarks>
        /// *Anonymous Access*
        /// </remarks>
        /// <returns>List of Service.</returns>
        /// <response code="200">List of Service successfully retrieved.</response>
        [AllowAnonymous]
        [ODataRoute]
        [ProducesResponseType(typeof(ODataValue<IEnumerable<ServiceViewDto>>), Status200OK)]
        [EnableQuery]
        public IQueryable<ServiceViewDto> Get()
        {
            return _context.Services
                .AsNoTracking()
                .ProjectTo<ServiceViewDto>(_mapper.ConfigurationProvider);
        }

        /// <summary>
        /// Gets a single Service.
        /// </summary>
        /// <remarks>
        /// *Anonymous Access*
        /// </remarks>
        /// <param name="id">The requested Service identifier.</param>
        /// <returns>The requested Service.</returns>
        /// <response code="200">The Service was successfully retrieved.</response>
        /// <response code="404">The Service does not exist.</response>
        [AllowAnonymous]
        [ODataRoute(IdRoute)]
        [ProducesResponseType(typeof(ServiceViewDto), Status200OK)]
        [ProducesResponseType(Status404NotFound)]
        [EnableQuery]
        public SingleResult<ServiceViewDto> Get(ushort id)
        {
            return SingleResult.Create(
                _context.Services
                    .AsNoTracking()
                    .Where(e => e.Id == id)
                    .ProjectTo<ServiceViewDto>(_mapper.ConfigurationProvider));
        }

        /// <summary>
        /// Creates a new Service.
        /// </summary>
        /// <remarks>
        /// *Min role: Admin*
        /// </remarks>
        /// <param name="create">The Service to create.</param>
        /// <returns>The created Service.</returns>
        /// <response code="201">The Service was successfully created.</response>
        /// <response code="204">The Service was successfully created.</response>
        /// <response code="400">The Service is invalid.</response>
        /// <response code="409">The Service with supplied id already exist.</response>
        [MultiRoleAuthorize(RoleEnum.JokizillaAdmin)]
        [ProducesResponseType(typeof(ServiceViewDto), Status201Created)]
        [ProducesResponseType(Status204NoContent)]
        [ProducesResponseType(Status400BadRequest)]
        [ProducesResponseType(Status409Conflict)]
        public async Task<IActionResult> Post([FromBody] ServiceUpdateDto create)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Service item = _mapper.Map<ServiceUpdateDto, Service>(create);
            _context.Services.Add(item);

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

            return Created(await _context.Services
                .AsNoTracking()
                .Where(e => e.Id == item.Id)
                .ProjectTo<ServiceViewDto>(_mapper.ConfigurationProvider)
                .SingleAsync());
        }

        /// <summary>
        /// Updates an existing Service.
        /// </summary>
        /// <remarks>
        /// *Min role: Admin*
        /// </remarks>
        /// <param name="id">The requested Service identifier.</param>
        /// <param name="delta">The partial Service to update.</param>
        /// <returns>The updated Service.</returns>
        /// <response code="200">The Service was successfully updated.</response>
        /// <response code="204">The Service was successfully updated.</response>
        /// <response code="400">The Service is invalid.</response>
        /// <response code="404">The Service does not exist.</response>
        [MultiRoleAuthorize(RoleEnum.JokizillaAdmin)]
        [ODataRoute(IdRoute)]
        [ProducesResponseType(typeof(ServiceViewDto), Status200OK)]
        [ProducesResponseType(Status204NoContent)]
        [ProducesResponseType(Status400BadRequest)]
        [ProducesResponseType(Status404NotFound)]
        public async Task<IActionResult> Patch(ushort id, [FromBody] Delta<ServiceUpdateDto> delta)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Service item = await _context.Services.FindAsync(id);

            if (item == null)
            {
                return NotFound();
            }

            ServiceUpdateDto update = _mapper.Map<Service, ServiceUpdateDto>(item);
            delta.Patch(update);
            _mapper.Map(update, item);
            await _context.SaveChangesAsync();
            return Updated(_mapper.Map<Service, ServiceUpdateDto>(item));
        }

        /// <summary>
        /// Deletes a Service.
        /// </summary>
        /// <remarks>
        /// *Min role: Admin*
        /// </remarks>
        /// <param name="id">The Service to delete.</param>
        /// <returns>None</returns>
        /// <response code="204">The Service was successfully deleted.</response>
        /// <response code="404">The Service does not exist.</response>
        [MultiRoleAuthorize(RoleEnum.JokizillaAdmin)]
        [ODataRoute(IdRoute)]
        [ProducesResponseType(Status204NoContent)]
        [ProducesResponseType(Status404NotFound)]
        public async Task<IActionResult> Delete(ushort id)
        {
            Service delete = await _context.Services.FindAsync(id);

            if (delete == null)
            {
                return NotFound();
            }

            _context.Services.Remove(delete);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private bool Exists(ushort id)
        {
            return _context.Services.Any(e => e.Id == id);
        }

        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
    }
}
