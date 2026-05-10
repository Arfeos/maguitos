using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneService :ISceneService
{
    private string lastScene;
    public void LoadScene(SceneNames scene)
    {
        lastScene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(scene.ToString());
    }
    public void GoBack()
    {
        if(!string.IsNullOrEmpty(lastScene))
            SceneManager.LoadScene(lastScene);
            
        
    }


}
