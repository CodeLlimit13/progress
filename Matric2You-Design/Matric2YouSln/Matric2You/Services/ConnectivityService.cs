using Microsoft.Maui.Networking;

namespace Matric2You.Services;

public interface IConnectivityService
{
 bool IsConnected { get; }
 event EventHandler<bool>? ConnectivityChanged;
}

public sealed class ConnectivityService : IConnectivityService, IDisposable
{
 private bool _disposed;

 public bool IsConnected => Connectivity.Current.NetworkAccess == NetworkAccess.Internet;

 public event EventHandler<bool>? ConnectivityChanged;

 public ConnectivityService()
 {
 Connectivity.ConnectivityChanged += OnConnectivityChanged;
 }

 private void OnConnectivityChanged(object? sender, ConnectivityChangedEventArgs e)
 {
 ConnectivityChanged?.Invoke(this, e.NetworkAccess == NetworkAccess.Internet);
 }

 public void Dispose()
 {
 if (_disposed) return;
 _disposed = true;
 Connectivity.ConnectivityChanged -= OnConnectivityChanged;
 }
}
