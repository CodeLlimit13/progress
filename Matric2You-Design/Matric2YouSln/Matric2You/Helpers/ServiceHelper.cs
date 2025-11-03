using Microsoft.Extensions.DependencyInjection;

namespace Matric2You.Helpers;

// Simple service locator for places we don't have direct DI
public static class ServiceHelper
{
 public static IServiceProvider? Services { get; private set; }

 public static void Initialize(IServiceProvider services)
 {
 Services = services;
 }

 public static T GetService<T>() where T : notnull
 {
 if (Services is null) throw new InvalidOperationException("Services not initialized");
 var service = Services.GetService<T>();
 return service ?? throw new InvalidOperationException($"Service not found: {typeof(T).Name}");
 }
}
