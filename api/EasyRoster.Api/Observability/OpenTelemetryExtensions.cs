using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
namespace EasyRoster.Api.Observability;
public static class OpenTelemetryExtensions {
 public static IServiceCollection AddApplicationTelemetry(this IServiceCollection services) {
  services.AddOpenTelemetry().WithTracing(t=>t.AddAspNetCoreInstrumentation().AddHttpClientInstrumentation()).WithMetrics(m=>m.AddAspNetCoreInstrumentation().AddHttpClientInstrumentation().AddRuntimeInstrumentation());
  return services;
 }
}
