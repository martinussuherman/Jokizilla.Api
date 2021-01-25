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
    /// Represents a RESTful service of Country.
    /// </summary>
    [Authorize]
    [Produces(JsonOutput)]
    [ODataRoutePrefix(nameof(Country))]
    [ApiVersionNeutral]
    public class CountryController : ODataController
    {
        /// <summary>
        /// Country REST service.
        /// </summary>
        /// <param name="context">Database context.</param>
        /// <param name="mapper">AutoMapper mapping profile.</param>
        public CountryController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        /// <summary>
        /// Retrieves list of Country.
        /// </summary>
        /// <remarks>
        /// *Anonymous Access*
        /// </remarks>
        /// <returns>List of Country.</returns>
        /// <response code="200">List of Country successfully retrieved.</response>
        [AllowAnonymous]
        [ODataRoute]
        [ProducesResponseType(typeof(ODataValue<IEnumerable<CountryViewDto>>), Status200OK)]
        [EnableQuery]
        public IQueryable<CountryViewDto> Get()
        {
            return _context.Countries
                .AsNoTracking()
                .ProjectTo<CountryViewDto>(_mapper.ConfigurationProvider);
        }

        /// <summary>
        /// Gets a single Country.
        /// </summary>
        /// <remarks>
        /// *Anonymous Access*
        /// </remarks>
        /// <param name="id">The requested Country identifier.</param>
        /// <returns>The requested Country.</returns>
        /// <response code="200">The Country was successfully retrieved.</response>
        /// <response code="404">The Country does not exist.</response>
        [AllowAnonymous]
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

        /// <summary>
        /// Creates a new Country.
        /// </summary>
        /// <remarks>
        /// *Min role: Admin*
        /// </remarks>
        /// <param name="create">The Country to create.</param>
        /// <returns>The created Country.</returns>
        /// <response code="201">The Country was successfully created.</response>
        /// <response code="204">The Country was successfully created.</response>
        /// <response code="400">The Country is invalid.</response>
        /// <response code="409">The Country with supplied id already exist.</response>
        [MultiRoleAuthorize(RoleEnum.JokizillaAdmin)]
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

        /// <summary>
        /// Updates an existing Country.
        /// </summary>
        /// <remarks>
        /// *Min role: Admin*
        /// </remarks>
        /// <param name="id">The requested Country identifier.</param>
        /// <param name="delta">The partial Country to update.</param>
        /// <returns>The updated Country.</returns>
        /// <response code="200">The Country was successfully updated.</response>
        /// <response code="204">The Country was successfully updated.</response>
        /// <response code="400">The Country is invalid.</response>
        /// <response code="404">The Country does not exist.</response>
        [MultiRoleAuthorize(RoleEnum.JokizillaAdmin)]
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

        /// <summary>
        /// Deletes a Country.
        /// </summary>
        /// <remarks>
        /// *Min role: Admin*
        /// </remarks>
        /// <param name="id">The Country to delete.</param>
        /// <returns>None</returns>
        /// <response code="204">The Country was successfully deleted.</response>
        /// <response code="404">The Country does not exist.</response>
        [MultiRoleAuthorize(RoleEnum.JokizillaAdmin)]
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
