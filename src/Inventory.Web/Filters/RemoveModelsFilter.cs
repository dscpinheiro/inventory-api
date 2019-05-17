using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Inventory.Web.Filters
{
    class RemoveModelsFilter : IDocumentFilter
    {
        public void Apply(SwaggerDocument swaggerDoc, DocumentFilterContext context)
        {
            swaggerDoc.Definitions.Remove(nameof(Models.Item));
            swaggerDoc.Definitions.Remove("ProblemDetails");
        }
    }
}