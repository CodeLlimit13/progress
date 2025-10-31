using System.Text.Json;
using Matric2You.Models;

namespace Matric2You.Services;

public interface IOfflineQueueService
{
 Task TrackProgressAsync(UpsertProgressRequest request, CancellationToken ct = default);
 Task TrySyncAsync(CancellationToken ct = default);
}

public sealed class OfflineQueueService : IOfflineQueueService, IDisposable
{
 private readonly IProgressApiService _api;
 private readonly IConnectivityService _connectivity;
 private readonly SemaphoreSlim _gate = new(1,1);
 private readonly string _queueFilePath;
 private readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web) { WriteIndented = false };
 private List<UpsertProgressRequest> _queue = new();
 private bool _initialized;
 private bool _disposed;

 public OfflineQueueService(IProgressApiService api, IConnectivityService connectivity)
 {
 _api = api;
 _connectivity = connectivity;
 _queueFilePath = Path.Combine(FileSystem.AppDataDirectory, "progress-queue.json");
 _connectivity.ConnectivityChanged += OnConnectivityChanged;
 }

 private async void OnConnectivityChanged(object? sender, bool online)
 {
 if (online)
 {
 try { await TrySyncAsync(); } catch { }
 }
 }

 private async Task EnsureLoadedAsync()
 {
 if (_initialized) return;
 try
 {
 if (File.Exists(_queueFilePath))
 {
 var json = await File.ReadAllTextAsync(_queueFilePath);
 var list = JsonSerializer.Deserialize<List<UpsertProgressRequest>>(json, _jsonOptions);
 _queue = list ?? new();
 }
 }
 catch { _queue = new(); }
 _initialized = true;
 }

 private async Task PersistAsync()
 {
 try
 {
 var json = JsonSerializer.Serialize(_queue, _jsonOptions);
 await File.WriteAllTextAsync(_queueFilePath, json);
 }
 catch { }
 }

 public async Task TrackProgressAsync(UpsertProgressRequest request, CancellationToken ct = default)
 {
 await _gate.WaitAsync(ct);
 try
 {
 await EnsureLoadedAsync();
 if (_connectivity.IsConnected)
 {
 var ok = await _api.UpsertProgressAsync(request, ct);
 if (ok) return;
 }
 _queue.Add(request);
 await PersistAsync();
 }
 finally
 {
 _gate.Release();
 }
 }

 public async Task TrySyncAsync(CancellationToken ct = default)
 {
 if (!_connectivity.IsConnected) return;
 await _gate.WaitAsync(ct);
 try
 {
 await EnsureLoadedAsync();
 if (_queue.Count ==0) return;
 var remaining = new List<UpsertProgressRequest>();
 foreach (var item in _queue)
 {
 if (ct.IsCancellationRequested) break;
 try
 {
 var ok = await _api.UpsertProgressAsync(item, ct);
 if (!ok) remaining.Add(item);
 }
 catch { remaining.Add(item); }
 }
 _queue = remaining;
 await PersistAsync();
 }
 finally
 {
 _gate.Release();
 }
 }

 public void Dispose()
 {
 if (_disposed) return;
 _disposed = true;
 _connectivity.ConnectivityChanged -= OnConnectivityChanged;
 _gate.Dispose();
 }
}
