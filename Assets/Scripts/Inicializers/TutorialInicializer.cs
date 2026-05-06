
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialInicializer : MonoBehaviour
{
    Workflow mainSceneWorkflow;

    [SerializeField] GameObject MessageBox;
    private void Awake()
    {
        InitWorkflow();
    }

    private void InitWorkflow()
    {
        mainSceneWorkflow = new Workflow(new List<IStep>
        {
            new MoveStep(),
            new PickUpSpellStep(),
            new PressSpaceStep()

        }, MessageBox);

        mainSceneWorkflow.OnComplete += WorkflowFinished;

        mainSceneWorkflow.Begin();

        
    }

    private void WorkflowFinished()
    {
        Debug.Log($"Hemos finalizado el workflow");
        MessageBox.SetActive(false);
    }
}
