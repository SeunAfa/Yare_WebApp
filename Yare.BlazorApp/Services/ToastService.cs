namespace Yare.BlazorApp.Services;

public enum ToastLevel { Success, Error, Info, Warning }

public class ToastMessage
{
    public Guid Id { get; } = Guid.NewGuid();
    public string Message { get; set; } = "";
    public ToastLevel Level { get; set; } = ToastLevel.Success;
}

public class ToastService
{
    public event Action? OnChange;
    public List<ToastMessage> Toasts { get; } = new();

    public void Success(string message) => Show(message, ToastLevel.Success);
    public void Error(string message) => Show(message, ToastLevel.Error);
    public void Info(string message) => Show(message, ToastLevel.Info);
    public void Warning(string message) => Show(message, ToastLevel.Warning);

    public void Show(string message, ToastLevel level = ToastLevel.Success)
    {
        var toast = new ToastMessage { Message = message, Level = level };
        Toasts.Add(toast);
        OnChange?.Invoke();
        _ = RemoveAfterDelay(toast);
    }

    public void Remove(ToastMessage toast)
    {
        if (Toasts.Remove(toast)) OnChange?.Invoke();
    }

    private async Task RemoveAfterDelay(ToastMessage toast)
    {
        await Task.Delay(3500);
        if (Toasts.Remove(toast)) OnChange?.Invoke();
    }
}
