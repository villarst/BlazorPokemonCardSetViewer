using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace BlazorPokemonCardSetViewer.Pages.ViewModels;

public class CounterViewModel : ReactiveObject, IDisposable
{
    private readonly ILogger<CounterViewModel> _logger;
    [Reactive] public int CurrentCount { get; set; }

    public CounterViewModel(ILogger<CounterViewModel> logger)
    {
        CurrentCount = 0;
        _logger = logger;
    }
    
    public void IncrementCount()
    {
        CurrentCount++;
    }
    
    public void Dispose()
    {
        CurrentCount = 0;
        _logger.LogInformation("Counter disposed");
    }
}