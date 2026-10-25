using Microsoft.AspNetCore.Mvc.Filters;

namespace CRUDPrograme.Filters
{
    public class PersonActionFilter : IActionFilter
    {

        private readonly ILogger<PersonActionFilter> _logger;

        public PersonActionFilter(ILogger<PersonActionFilter> logger)
        {
            _logger = logger;
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            _logger.LogInformation("[Action]--{FilterName}.{method} is run",
                nameof(PersonActionFilter), nameof(OnActionExecuted));
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.ActionArguments.ContainsKey("ioo"))
            {
                _logger.LogInformation("[Action]--{FilterName}.{method} is run",
                    nameof(PersonActionFilter), nameof(OnActionExecuting));
            }
            else
            {
                _logger.LogInformation("[Action]--{FilterName}.{method} is down",
                    nameof(PersonActionFilter), nameof(OnActionExecuting));
            }


        }
    }
}
