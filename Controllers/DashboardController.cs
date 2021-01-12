using System.Collections.Generic;
using System.Linq;
using Jokizilla.Models.Models;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Routing;
using Microsoft.AspNetCore.Mvc;
using static Jokizilla.Api.Misc.ApiInfo;
using static Microsoft.AspNetCore.Http.StatusCodes;

namespace Jokizilla.Api.Controllers
{
    [Produces(JsonOutput)]
    // [ODataRoutePrefix("Dashboard")]
    [ApiVersionNeutral]
    public class DashboardController : ODataController
    {
        public DashboardController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [ODataRoute(nameof(TestCount))]
        [Produces(JsonOutput)]
        [ProducesResponseType(typeof(long), Status200OK)]
        public long TestCount()
        {
            return 0;
        }

        // [HttpGet(nameof(ActivityLog))]
        // [ProducesResponseType(typeof(ODataValue<IEnumerable<ActivityLog>>), Status200OK)]
        // [EnableQuery]
        // public IQueryable<ActivityLog> ActivityLog()
        // {
        //     return _context.ActivityLogs;
        // }

        private readonly AppDbContext _context;
    }
}