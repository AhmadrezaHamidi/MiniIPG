using System.Diagnostics;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace PaymentService.Application.Common.Behaviours;

public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;
        private static readonly string Source = typeof(TRequest).Name;
        private static readonly string ServiceName = "LoggingBehavior";

        public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            string requestId = Guid.NewGuid().ToString();
            string requestName = typeof(TRequest).Name;

            try
            {
                _logger.LogInformation($"Started handling {requestName} (ID: {requestId})");

                var requestJson = SanitizeRequest(request);

                _logger.LogInformation($"Request details: {requestJson}");

                var sw = Stopwatch.StartNew();

                var response = await next();

                sw.Stop();

                _logger.LogInformation($"Completed handling {requestName} (ID: {requestId}) in {sw.ElapsedMilliseconds}ms");

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error handling {requestName} (ID: {requestId}): {ex.Message}");

                throw; 
            }
        }
        private object SanitizeRequest(object request)
        {
            var props = request.GetType().GetProperties()
                .Where(p => p.CanRead && !typeof(Stream).IsAssignableFrom(p.PropertyType) && p.PropertyType != typeof(IFormFile))
                .ToDictionary(p => p.Name, p => p.GetValue(request));

            return props;
        }
    }
