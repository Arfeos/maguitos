
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialInicializer : MonoBehaviour
{
    Workflow mainSceneWorkflow;

    [SerializeField] GameObject MessageBox;
    [SerializeField] GameObject door;
    private void Awake()
    {
        InitWorkflow();
    }

    private void InitWorkflow()
    {
        mainSceneWorkflow = new Workflow(new List<IStep>
        {
            new CameraMoveStep(),
            new MoveStep(),
            new PickUpSpellStep(),
            new ShootStep(),
            new OpenDoorStep(door),
            new CrouchStep(),
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
