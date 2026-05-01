using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public interface ISceneService
{
    //Guardar ultima escena y cambiarla
    void SaveScene(Scene oldScene, Scene newScene);
    void LoadScene(SceneNames scene);
    void GoBack();
}
