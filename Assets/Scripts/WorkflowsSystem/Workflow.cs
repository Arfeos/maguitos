using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using static UnityEngine.Rendering.DebugUI;

public class Workflow
{
    private List<IStep> _steps = new List<IStep>();
    private IStep _currentStep = null;

    IAnimationService _animation;
    IAlertService _alertService;
    public event Action OnComplete;
    private GameObject _messageBox;
    public Workflow(List<IStep> workflowSteps, GameObject MessageBox)
    {
        this._steps = workflowSteps;
        _messageBox = MessageBox;

        _alertService = AppContainer.Get<IAlertService>();

        _animation = AppContainer.Get<IAnimationService>();
    }

    public void Begin()
    {
        // Comprobamos que el workflow no está iniciado
        if (this._currentStep != null)
            return;

        // Comprobamos si hay steps
        if (this._steps.Count == 0)
            return;

        // Nos suscribimos al cambio de idioma
        LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;

        this.ActivateStep(this._steps[0]);
    }

    private void OnLocaleChanged(Locale locale)
    {
        // Si hay un step activo, refrescamos el MessageBox con el nuevo idioma
        if (_currentStep == null) return;

        ObjectDataScriptable dataStep = ScriptableObject.CreateInstance<ObjectDataScriptable>();
        dataStep.objectName = _currentStep.Name;
        dataStep.objetDescription = _currentStep.Description;

        _alertService.ShowAlertMessage(_messageBox, dataStep);
    }

    private void ActivateStep(IStep step)
    {
        if (step == null)
            return;

        // Establecemos cual es el step actual
        this._currentStep = step;

        // Activamos el step
        this._currentStep.Activate();

        this._currentStep.OnComplete += StepComplete;

        //_animation.FadeOutUIAnimation(_messageBox, 1);
        _animation.FadeInUIAnimation(_messageBox, 1);
        //_messageBox.SetActive(true);
        ObjectDataScriptable dataStep = ScriptableObject.CreateInstance<ObjectDataScriptable>();
        dataStep.objectName = _currentStep.Name;
        dataStep.objetDescription = _currentStep.Description;

        _alertService.ShowAlertMessage(_messageBox, dataStep);
    }

    private void DeactivateCurrentStep()
    {
        this._currentStep.OnComplete -= StepComplete;
    }

    private void StepComplete()
    {
        // Obtenemos la posici�n del step actual en la lista
        var indexOfCurrentStep = this._steps.IndexOf(this._currentStep);

        // Si no encontramos el step salimos
        if (indexOfCurrentStep == -1)
        {
            Debug.LogError($"No se encuentra el step {this._currentStep.Name}");
            return;
        }

        // Si no hay mas steps, workflow completado
        if (indexOfCurrentStep == this._steps.Count - 1)
        {
            LocalizationSettings.SelectedLocaleChanged -= OnLocaleChanged;

            this.OnComplete?.Invoke();
            this.DeactivateCurrentStep();

            return;
        }

        // Obtenemos el siguiente step del workflow
        var nextStep = this._steps[indexOfCurrentStep + 1];

        // Desactivamos el currentStep
        this.DeactivateCurrentStep();

        // Activamos el siguiente step
        this.ActivateStep(nextStep);
    }

    public IStep getCurrentStep()
    {
        return this._currentStep;
    }
}
