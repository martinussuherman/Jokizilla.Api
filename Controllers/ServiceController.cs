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
    [ODataRoutePrefix(nameof(Service))]
    [ApiVersionNeutral]
    public class ServiceController : ODataController
    {
        public ServiceController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [ODataRoute]
        [ProducesResponseType(typeof(ODataValue<IEnumerable<ServiceViewDto>>), Status200OK)]
        [EnableQuery]
        public IQueryable<ServiceViewDto> Get()
        {
            return _context.Services
                .AsNoTracking()
                .ProjectTo<ServiceViewDto>(_mapper.ConfigurationProvider);
        }

        [ODataRoute(IdRoute)]
        [ProducesResponseType(typeof(ServiceViewDto), Status200OK)]
        [ProducesResponseType(Status404NotFound)]
        [EnableQuery(AllowedQueryOptions = AllowedQueryOptions.Select)]
        public SingleResult<ServiceViewDto> Get(ushort id)
        {
            return SingleResult.Create(
                _context.Services
                    .AsNoTracking()
                    .Where(e => e.Id == id)
                    .ProjectTo<ServiceViewDto>(_mapper.ConfigurationProvider));
        }

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

        [ODataRoute(IdRoute)]
        [ProducesResponseType(Status204NoContent)]
        [ProducesResponseType(Status404NotFound)]
        public async Task<IActionResult> Delete(ushort id)
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

        private bool Exists(ushort id)
        {
            return _context.Services.Any(e => e.Id == id);
        }

        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
    }
}
