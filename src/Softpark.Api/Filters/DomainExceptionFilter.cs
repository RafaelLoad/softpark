using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Softpark.Domain.Exceptions;

namespace Softpark.Api.Filters;

public class DomainExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        if (context.Exception is NotFoundException notFound)
        {
            context.Result = new NotFoundObjectResult(new { mensagem = notFound.Message });
            context.ExceptionHandled = true;
        }
        else if (context.Exception is DomainException domain)
        {
            context.Result = new BadRequestObjectResult(new { mensagem = domain.Message });
            context.ExceptionHandled = true;
        }
    }
}
