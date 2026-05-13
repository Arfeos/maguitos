using UnityEngine;
using UnityEngine.InputSystem;

public class LanternCollect : MonoBehaviour, ICollectable
{
    private bool equipped = false;

    private Light lanternLight;

    private Transform cameraTransform;

    // Rotación visual de la linterna respecto a la cámara
    [SerializeField]
    private Vector3 rotationOffset = new Vector3(75f, 0f, 0f);

    private void Start()
    {
        lanternLight = GetComponentInChildren<Light>();

        PlayerInputManager.Actions.Player.Lantern.performed += TurnLantern;
    }

    private void LateUpdate()
    {
        if (!equipped || cameraTransform == null)
            return;

        SetRotation();
    }

    private void SetRotation()
    {
        Quaternion offset = Quaternion.Euler(rotationOffset);

        // Igualar la rotación GLOBAL de la cámara
        // aunque la linterna tenga otro padre (hueso cabeza)
        transform.rotation = cameraTransform.rotation * offset;
    }

    private void TurnLantern(InputAction.CallbackContext ctx)
    {
        if (!equipped || lanternLight == null)
            return;

        lanternLight.enabled = !lanternLight.enabled;
    }

    public void Collect()
    {
        PlayerController player = FindAnyObjectByType<PlayerController>();

        if (player == null)
            return;

        cameraTransform = player.GetComponentInChildren<Camera>().transform;

        SetPosition(player.transform);

        DisableRender();

        equipped = true;
    }

    private void SetPosition(Transform player)
    {
        lanternSocket socket =
            player.GetComponentInChildren<lanternSocket>();

        if (socket == null)
            return;

        transform.SetParent(socket.transform);

        transform.localPosition = Vector3.zero;

        // La rotación la controla LateUpdate()
        transform.localRotation = Quaternion.identity;

        Collider col = GetComponent<Collider>();

        if (col != null)
            col.enabled = false;
    }

    private void DisableRender()
    {
        MeshRenderer[] renderers =
            GetComponentsInChildren<MeshRenderer>();

        foreach (MeshRenderer r in renderers)
        {
            r.enabled = false;
        }
    }

    private void OnDestroy()
    {
        PlayerInputManager.Actions.Player.Lantern.performed -= TurnLantern;
    }
}