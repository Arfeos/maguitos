using System;

public interface IStep
{
    // Nombre del Step
    public string Name { get; }

    // Descripción del Step
    public string Description { get; }

    // Indicador para ver si está completo o no
    public bool IsComplete { get; set; }

    // Método para activar el step
    public void Activate();

    // Método para desactivar el step
    public void Deactivate();

    // Evento invocado cuando se completa la acción
    public event Action OnComplete;
}
