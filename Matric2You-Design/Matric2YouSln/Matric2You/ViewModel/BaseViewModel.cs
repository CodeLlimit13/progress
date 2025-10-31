using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Matric2You.ViewModel;

public abstract class BaseViewModel : INotifyPropertyChanged
{
 public event PropertyChangedEventHandler? PropertyChanged;

 protected bool SetProperty<T>(ref T backing, T value, [CallerMemberName] string? propertyName = null)
 {
 if (EqualityComparer<T>.Default.Equals(backing, value)) return false;
 backing = value;
 OnPropertyChanged(propertyName);
 return true;
 }

 protected void OnPropertyChanged([CallerMemberName] string? name = null)
 => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

 private bool _isBusy;
 public bool IsBusy { get => _isBusy; set => SetProperty(ref _isBusy, value); }

 private string? _error;
 public string? Error { get => _error; set => SetProperty(ref _error, value); }
}
