namespace WorldCupPlanner.UI.Data;

public enum TipoToast { Exito, Error, Advertencia, Info }

public record ToastMensaje(string Mensaje, TipoToast Tipo, Guid Id)
{
    public ToastMensaje(string mensaje, TipoToast tipo) : this(mensaje, tipo, Guid.NewGuid()) { }
}

public class ToastServicio
{
    public event Action<ToastMensaje>? OnToast;

    public void Exito(string mensaje)      => OnToast?.Invoke(new ToastMensaje(mensaje, TipoToast.Exito));
    public void Error(string mensaje)      => OnToast?.Invoke(new ToastMensaje(mensaje, TipoToast.Error));
    public void Advertencia(string mensaje) => OnToast?.Invoke(new ToastMensaje(mensaje, TipoToast.Advertencia));
    public void Info(string mensaje)       => OnToast?.Invoke(new ToastMensaje(mensaje, TipoToast.Info));
}
