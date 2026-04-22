using UnityEngine;
using UnityEngine.SceneManagement;

public interface ISceneService
{
    public void LoadScene(string sceneName);
    public void GoBack();
    public void SaveScene(Scene oldScene, Scene newScene);
}
