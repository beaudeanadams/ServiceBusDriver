using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure.Identity;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using ServiceBusDriver.Shared.Features.Error;
using ServiceBusDriver.Shared.Tools;
using AppExceptionFactory = ServiceBusDriver.Server.Services.Exceptions.AppExceptionFactory;

namespace ServiceBusDriver.Server.Middlewares
{
    public class GlobalExceptionMiddleware
    {
        private readonly ILogger<GlobalExceptionMiddleware> _logger;
        private readonly RequestDelegate _next;
        private string _traceId;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                _traceId = context.Request.Headers.FirstOrDefault(x => x.Key == "TraceId").Value.FirstOrDefault();
                if (string.IsNullOrWhiteSpace(_traceId))
                {
                    _traceId = Guid.NewGuid().ToString();
                }

                _logger.LogInformation("Invoking Path - {0}", context.Request.Path);
                _logger.LogInformation("Setting Trace Id - {0}", _traceId);


                await _next(context).ConfigureAwait(false);
            }

            catch (Exception exception)
            {
                _logger.LogError("Error Occured : " + exception.ToFullExceptionString());
                AppErrorMessageDto errorMessage = null;
                var statusCode = StatusCodes.Status500InternalServerError;

                if (exception is AppException appStandardException)
                {
                    errorMessage = appStandardException.ErrorMessage ??
                                   AppExceptionFactory.CreateServerCommunicationException();

                    statusCode = appStandardException.HttpStatusCode ?? StatusCodes.Status500InternalServerError;

                    errorMessage.SetException(exception);
                    _logger.LogError(errorMessage.Code, errorMessage.GetException(), errorMessage.UserMessageText);
                }
                else if (exception is ValidationException validationException)
                {
                    errorMessage = AppExceptionFactory.CreateBadRequestException();
                    if (validationException.Errors.Any())
                    {
                        var errors = validationException.Errors.Select(err => new
                        {
                            err.PropertyName,
                            err.ErrorMessage
                        });
                        errorMessage.UserMessageText = JsonConvert.SerializeObject(errors);
                    }
                    else
                    {
                        errorMessage.UserMessageText = validationException.Message;
                    }
                    errorMessage.SetException(exception);
                    _logger.LogError(errorMessage.Code, errorMessage.GetException(), errorMessage.UserMessageText);
                    statusCode = StatusCodes.Status400BadRequest;
                }
                else if (exception is AuthenticationFailedException authenticationFailedException)
                {
                    errorMessage = AppExceptionFactory.CreateAuthenticationError(exception.Message);
                    _logger.LogError(errorMessage.Code, errorMessage.GetException(), errorMessage.UserMessageText);
                    statusCode = StatusCodes.Status401Unauthorized;
                }
                else
                {
                    errorMessage = AppExceptionFactory.CreateServerCommunicationException();
                    errorMessage.SetException(exception);
                    _logger.LogError(errorMessage.Code, errorMessage.GetException(), errorMessage.UserMessageText);
                }

                // We can't do anything if the response has already started, just abort.
                if (context.Response.HasStarted) throw;

                try
                {
                    context.Response.StatusCode = statusCode;
                    await WriteErrorResponseAsync(context, errorMessage).ConfigureAwait(false);
                }
                catch (Exception)
                {
                }
            }
        }

        private async Task WriteErrorResponseAsync(HttpContext context, AppErrorMessageDto errorModel)
        {
            if (errorModel != null)
            {
                errorModel.SupportReferenceId = _traceId;
            }

            await WriteJsonResponseAsync(context, errorModel).ConfigureAwait(false); // Default to JSON
        }

        private async Task WriteJsonResponseAsync<TErrorResponseObject>(HttpContext context,
            TErrorResponseObject responseObject)
        {
            // Set headers
            context.Response.Headers.Add(HeaderNames.ContentType, "application/json");

            // Create Write Context
            var writeContext = new OutputFormatterWriteContext(
                context,
                (stream, encoding) => new StreamWriter(stream, encoding),
                responseObject.GetType(),
                responseObject);

            await writeContext.HttpContext.Response.WriteAsync(
                JsonConvert.SerializeObject(responseObject),
                Encoding.UTF8,
                writeContext.HttpContext.RequestAborted).ConfigureAwait(false);
        }
    }
}