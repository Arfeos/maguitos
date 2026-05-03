using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneService : MonoBehaviour, ISceneService
{
    private static Stack<string> sceneHistory = new Stack<string>();
    private void OnEnable()
    {
        SceneManager.activeSceneChanged += SaveScene;
    }
    private void OnDisable()
    {
        SceneManager.activeSceneChanged -= SaveScene;
    }
    public void LoadScene(SceneNames scene)
    {
        SceneManager.LoadScene(scene.ToString());
    }
    public void GoBack()
    {
        if (sceneHistory.Count > 0)
        {
            SceneManager.LoadScene(sceneHistory.Pop());
            
        }
    }

    public void SaveScene(Scene oldScene, Scene newScene)
    {
        sceneHistory.Push(oldScene.name);
    }
}
