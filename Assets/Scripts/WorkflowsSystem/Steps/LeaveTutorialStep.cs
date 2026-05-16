using System;
using UnityEngine;
using UnityEngine.Localization;

public class LeaveTutorialStep : IStep
{

    // --- Variables ---
    private bool _isComplete = false;


    // --- IStep ---
    public LocalizedString Name { get => new LocalizedString { TableReference = "Steps", TableEntryReference = "LeaveTutorialName" }; }
    public LocalizedString Description
    {
        get => new LocalizedString { TableReference = "Steps", TableEntryReference = "LeaveTutorialDesc" };
    }

    public bool IsComplete { get => this._isComplete; set => this._isComplete = value; }
    public event Action OnComplete;
    private Portal_Controller _portal;
    public LeaveTutorialStep(Portal_Controller portal)
    {
        _portal = portal;
    }

    public void Activate()
    {
        Debug.Log($"Activamos {this.Name}");
        Debug.Log($"{this.Description}");
        _portal.TogglePortal(true);
    }

    public void Deactivate()
    {
    }
}
