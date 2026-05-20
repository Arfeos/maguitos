using UnityEngine;

public class QuitButton : BaseButton
{
    public void Quit()
    {
        Debug.Log("Saliendo del juego...");

        Application.Quit();
    }
}