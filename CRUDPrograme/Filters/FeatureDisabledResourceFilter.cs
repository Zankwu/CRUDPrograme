using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CRUDPrograme.Filters
{
    public class FeatureDisabledResourceFilter : IAsyncResourceFilter
    {
        private readonly ILogger<IAsyncActionFilter> _logger;
        private readonly bool _isDisabled;

        public FeatureDisabledResourceFilter(ILogger<IAsyncActionFilter> logger,bool isDisabled)
        {
            _logger = logger;
            _isDisabled = isDisabled;
        }



        public async Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
        {
            _logger.LogInformation("[OnResourceExecutionAsync]{FilterName}.{MethodName} method--------ing",
                nameof(FeatureDisabledResourceFilter), nameof(OnResourceExecutionAsync));

            if (context.HttpContext.Request.Query["Auth"] == "zank" || _isDisabled)
            {
                await next();
                _logger.LogInformation("[OnResourceExecutionAsync]{FilterName}.{MethodName} method--------done",
                    nameof(FeatureDisabledResourceFilter), nameof(OnResourceExecutionAsync));
            }
            else 
            {
                context.Result = new StatusCodeResult(StatusCodes.Status500InternalServerError);
                return;
            }
            
        }
    }
}
