using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Serilog.Core;

namespace CRUDPrograme.Filters
{
    public class ResponseHeaderAsyncActionFilter : IAsyncActionFilter, IOrderedFilter
    {
        private readonly ILogger<IAsyncActionFilter> _logger;
        private readonly string _key;
        private readonly string _value;

        public int Order { get; set; }

        //public ResponseHeaderActionFilter(ILogger<ResponseHeaderActionFilter> logger,string key,string value)
        //{
        //    _logger = logger;
        //    _key = key;
        //    _value = value;
        //}
        public ResponseHeaderAsyncActionFilter(string key, string value, int _order, ILogger<ResponseHeaderAsyncActionFilter> logger)
        {
            _logger = logger;
            _key = key;
            _value = value;
            Order = _order;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            _logger.LogInformation("{FilterName}.{MethodName} method",
                nameof(ResponseHeaderAsyncActionFilter), nameof(OnActionExecutionAsync) + "Auth OK?");
            _logger.LogInformation("{order}.{method} --------------ORDER XXX",
            Order, _value);

            if (!context.HttpContext.Request.Query.ContainsKey("auth"))
            {
                _logger.LogInformation("{FilterName}.{MethodName} method",
                    nameof(ResponseHeaderAsyncActionFilter), nameof(OnActionExecutionAsync) + "Auth GG");
                context.Result = new UnauthorizedResult();
                return;
            }
            await next();
            _logger.LogInformation("{FilterName}.{MethodName} method",
            nameof(ResponseHeaderAsyncActionFilter), nameof(OnActionExecutionAsync) +"Auth OK");
        }



        //public void OnActionExecuted(ActionExecutedContext context)
        //{
        //    _logger.LogInformation("{FilterName}.{MethodName} method",
        //    nameof(ResponseHeaderActionFilter), nameof(OnActionExecuted));

        //}

        //public void OnActionExecuting(ActionExecutingContext context)
        //{
        //    _logger.LogInformation("{FilterName}.{MethodName} method",
        //    nameof(ResponseHeaderActionFilter), nameof(OnActionExecuting));
        //    _logger.LogInformation("{order}.{method} --------------ORDER XXX",
        //    Order, _value);

        //    if (context.HttpContext.Request.Headers.ContainsKey("Auth"))
        //    {

        //    }
        //}
    }
}
