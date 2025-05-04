using ReactiveUI;

namespace BlazorPokemonCardSetViewer.Pages.ViewModels;

public class CounterViewModel : ReactiveObject, IDisposable
{
    public int CurrentCount { get; set; }

    public CounterViewModel()
    {
        CurrentCount = 0;
    }
    
    private void IncrementCount()
    {
        CurrentCount++;
    }
    
    public void Dispose()
    {
        CurrentCount = 0;
    }
}