using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public interface ISceneService
{

    void LoadScene(SceneNames scene);
    void GoBack();
}
