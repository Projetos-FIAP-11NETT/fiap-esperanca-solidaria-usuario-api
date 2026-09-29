using FiapEsperancaSolidaria.Usuario.Application.Common;
using FiapEsperancaSolidaria.Usuario.Shared.Abstractions;
using MediatR;
using Microsoft.Extensions.Logging;
using Serilog.Context;
using System.Diagnostics;

namespace FiapEsperancaSolidaria.Usuario.Application.Behaviors;

public sealed class LoggingBehavior<TRequest, TResponse>
    (
        ILogger<LoggingBehavior<TRequest, TResponse>> logger,
        ICorrelationIdAccessor correlation//,
        //IObservabilityService observabilityService
    )
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var correlationId = correlation.CorrelationId;
        var stopwatch = Stopwatch.StartNew();
        var safeRequest = DataMasker.Mask(request!);

        using (LogContext.PushProperty("CorrelationId", correlationId))
        {
            //observabilityService.AddCustomAttribute("CorrelationId", correlationId);
            //observabilityService.AddCustomAttribute("RequestName", typeof(TRequest).Name);
            //observabilityService.AddCustomAttribute("request.payload", safeRequest);
        }
        // REQUEST
        logger.LogInformation(
            "CorrelationId {CorrelationId} | Iniciando {RequestName} Parametros: {request.payload}",
            correlationId,
            requestName,
            safeRequest
        );

        try
        {
            var response = await next();

            //observabilityService.AddCustomAttribute("response.payload", response!);

            stopwatch.Stop();

            // RESPONSE
            logger.LogInformation(
                "CorrelationId {CorrelationId} | Finalizando {RequestName} em {Elapsed}ms Response: {response.payload}",
                correlationId,
                requestName,
                stopwatch.ElapsedMilliseconds,
                response
            );

            return response;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();

            logger.LogError(
                ex,
                "CorrelationId {CorrelationId} | Erro em {RequestName} em {Elapsed}ms",
                correlationId,
                requestName,
                stopwatch.ElapsedMilliseconds
            );

            throw;
        }
    }
}
