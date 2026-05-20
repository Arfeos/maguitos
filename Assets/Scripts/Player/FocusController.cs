using System.Collections;
using UnityEngine;

public class FocusController : MonoBehaviour
{
    private Animator pointAnimator;
    private Camera playerCamera;
    private Animator cameraAnimator;

    private Coroutine currentRoutine;
    private int cameraIgnoreLayer;
    void Start()
    {
        pointAnimator = GetComponent<Animator>();
        playerCamera = Camera.main;
        cameraAnimator = playerCamera.GetComponent<Animator>();

        cameraIgnoreLayer = LayerMask.NameToLayer("CameraIgnore");
    }

    // Update is called once per frame
    void Update()
    {
        Focus();
    }
    void Focus()
    {
        var pI = PlayerInputManager.Actions.Player.Focus.IsPressed();
        pointAnimator.SetBool("Focus", pI);
        cameraAnimator.SetBool("Focus", pI);


        if (PlayerInputManager.Actions.Player.Focus.IsPressed())
        {
            if (currentRoutine == null)
            {
                currentRoutine = StartCoroutine(ChangeCullingAfterDelay(false));
            }
        }
        else
        {
            if (currentRoutine == null)
            {
                currentRoutine = StartCoroutine(ChangeCullingAfterDelay(true));
            }
        }
    }
    

    private IEnumerator ChangeCullingAfterDelay(bool enableLayer)
    {
        yield return new WaitForSeconds(0.1f);

        if (enableLayer)
        {
            // Mostrar layer
            playerCamera.cullingMask |= (1 << cameraIgnoreLayer);
        }
        else
        {
            // Ocultar layer
            playerCamera.cullingMask &= ~(1 << cameraIgnoreLayer);
        }

        currentRoutine = null;
    }
}
