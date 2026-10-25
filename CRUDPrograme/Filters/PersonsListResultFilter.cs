using Microsoft.AspNetCore.Mvc.Filters;
using System.Diagnostics;

namespace CRUDPrograme.Filters
{
    public class PersonsListResultFilter : IAsyncResultFilter
    {

        private readonly ILogger<PersonsListResultFilter> _logger;

        public PersonsListResultFilter(ILogger<PersonsListResultFilter> logger)
        {
            _logger = logger;
        }

        public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            _logger.LogInformation("[Result Filter Begin]=====>{class}.{method}", 
                nameof(PersonsListResultFilter), nameof(OnResultExecutionAsync));
            Stopwatch stopwatch = Stopwatch.StartNew();

            await next();

            _logger.LogInformation("[Result Filter End]=====>{class}.{method}",
                nameof(PersonsListResultFilter), nameof(OnResultExecutionAsync));

            stopwatch.Stop();
            // 向响应头追加最后修改时间戳
            _logger.LogInformation("[Result Filter End]=====>TimeOut={TimeOut}.StatusCode={StatusCode}Type.{Type}",
                stopwatch.Elapsed, context.HttpContext.Response.StatusCode,context.Result);
        }
    }
}
