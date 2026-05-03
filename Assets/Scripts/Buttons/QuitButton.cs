using UnityEngine;

public class QuitButton : MonoBehaviour
{
    public void Quit()
    {
        Debug.Log("Saliendo del juego...");

        Application.Quit();
    }
}