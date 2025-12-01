using MediatR;
using StargateAPI.Business.Data;

namespace StargateAPI.Business.Behaviors;

public class LoggingBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;
    private readonly StargateContext _context;

    public LoggingBehavior(
        ILogger<LoggingBehavior<TRequest, TResponse>> logger,
        StargateContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var guid = Guid.NewGuid();

        try
        {
            _logger.LogInformation("Handling {Request}: {Guid}", typeof(TRequest).Name, guid);
            await LogToDatabaseAsync(guid.ToString(), $"Handling {typeof(TRequest).Name}");

            var response = await next();
            
            _logger.LogInformation("Handled {Request}: {Guid}", typeof(TRequest).Name, guid);
            await LogToDatabaseAsync(guid.ToString(), $"Handled {typeof(TRequest).Name}");
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling {Request}: {Guid}", typeof(TRequest).Name, guid);
            await LogToDatabaseAsync(guid.ToString(), $"Error handling {typeof(TRequest).Name}: {ex.Message}", ex.StackTrace, true);
            throw;
        }
    }

    private async Task LogToDatabaseAsync(string groupIdentifier, string message, string? data = null, bool isException = false)
    {
        try
        {
            var logEntry = new Log
            {
                GroupIdentifier = groupIdentifier,
                Message = message,
                CreatedAt = DateTime.UtcNow,
                Data = data,
                IsException = isException
            };

            _context.Logs.Add(logEntry);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to log to database: {groupIdentifier}", groupIdentifier);
        }
    }
}
