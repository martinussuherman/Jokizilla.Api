using Jokizilla.Api.Controllers;
using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNetCore.Mvc;

namespace Jokizilla.Api.Configuration
{
    /// <summary>
    /// Represents the model configuration for Dashboard.
    /// </summary>
    public class DashboardInfoConfiguration : IModelConfiguration
    {
        //     ComplexTypeConfiguration<DashboardInfo> info = builder
        //         .ComplexType<DashboardInfo>();
        //     builder.Function(nameof(DashboardInfoController.DashboardPemohon))
        //         .Returns<DashboardInfo>();

        /// <inheritdoc />
        public void Apply(ODataModelBuilder builder, ApiVersion apiVersion, string routePrefix)
        {
            builder.Function(nameof(DashboardController.TestCount))
                .Returns<long>();
        }
    }
}
