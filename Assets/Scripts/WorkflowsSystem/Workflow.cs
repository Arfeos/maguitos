using System;
using System.Collections.Generic;
using UnityEngine;

public class Workflow
{
    private List<IStep> _steps = new List<IStep>();
    private IStep _currentStep = null;
    
    public event Action OnComplete;

    public Workflow(List<IStep> workflowSteps)
    {
        this._steps = workflowSteps;
    }

    public void Begin()
    {
        // Comprobamos que el workflow no esté iniciado
        if (this._currentStep != null)
            return;

        // Comprobamos si hay steps
        if (this._steps.Count == 0)
            return;

        this.ActivateStep(this._steps[0]);
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
    }

    private  void DeactivateCurrentStep()
    {
        this._currentStep.OnComplete -= StepComplete;
    }

    private void StepComplete()
    {
        // Obtenemos la posición del step actual en la lista
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
            this.OnComplete?.Invoke();
            return;
        }

        // Obtenemos el siguiente step del workflow
        var nextStep = this._steps[indexOfCurrentStep + 1];

        // Desactivamos el currentStep
        this.DeactivateCurrentStep();

        // Activamos el siguiente step
        this.ActivateStep(nextStep);
    }
}
