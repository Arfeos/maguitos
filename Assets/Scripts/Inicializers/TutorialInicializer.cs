
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialInicializer : MonoBehaviour
{
    Workflow mainSceneWorkflow;
    private void Awake()
    {
        InitWorkflow();
    }

    private void InitWorkflow()
    {
        mainSceneWorkflow = new Workflow(new List<IStep>
        {
            new PressAStep(),
            new PressSpaceFiveTimesStep()

        });

        mainSceneWorkflow.OnComplete += WorkflowFinished;

        mainSceneWorkflow.Begin();
    }

    private void WorkflowFinished()
    {
        Debug.Log($"Hemos finalizado el workflow");

    }
}
