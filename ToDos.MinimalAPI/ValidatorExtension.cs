using FluentValidation;

namespace ToDos.MinimalAPI;

public static class ValidatorExtension
{
    public static RouteHandlerBuilder WithValidator<T>(this RouteHandlerBuilder builder)
        where T : class
    {
        builder.Add(endpointBuilder =>
        {
            var originalDelegate = endpointBuilder.RequestDelegate;
            endpointBuilder.RequestDelegate = async httpContext =>
            {
                var validator = httpContext.RequestServices.GetRequiredService<IValidator<T>>();

                httpContext.Request.EnableBuffering();
                var body = await httpContext.Request.ReadFromJsonAsync<T>();

                if (body is null)
                {
                    httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                    await httpContext.Response.WriteAsync("Couldn't map body to request model");
                    return;
                }

                var validationResult = validator.Validate(body);

                if (!validationResult.IsValid)
                {
                    httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                    await httpContext.Response.WriteAsJsonAsync(validationResult.Errors);
                    return;
                }

                httpContext.Request.Body.Position = 0;
                await originalDelegate(httpContext);
            };
        });

        return builder;
    }
}
