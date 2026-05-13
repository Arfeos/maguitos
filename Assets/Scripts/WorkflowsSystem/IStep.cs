using System;
using UnityEngine.Localization;

public interface IStep
{
    // Nombre del Step
    public LocalizedString Name { get; }

    // Descripci�n del Step
    public LocalizedString Description { get; }

    // Indicador para ver si est� completo o no
    public bool IsComplete { get; set; }

    // M�todo para activar el step
    public void Activate();

    // M�todo para desactivar el step
    public void Deactivate();

    // Evento invocado cuando se completa la acci�n
    public event Action OnComplete;
}
