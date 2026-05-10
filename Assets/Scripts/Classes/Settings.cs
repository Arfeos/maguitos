using UnityEngine;
[System.Serializable]
public class Settings
{
   public Languages language;
    public int axisXDirection;
    public int axisYDirection;
    public float musicVolume;
    public float masterVolume;
    public float sensibility;
    public Settings()
    {
        language = Languages.English;
        axisXDirection = 1;
        axisYDirection = 1;
        musicVolume = 1f;
        masterVolume = 1f;
        sensibility = 1f;
    }
    public Settings(Languages language, int axisXDirection, int axisYDirection, float musicVolume, float masterVolume, float sensibility)
    {
        this.language = language;
        this.axisXDirection = axisXDirection;
        this.axisYDirection = axisYDirection;
        this.musicVolume = musicVolume;
        this.masterVolume = masterVolume;
        this.sensibility = sensibility;
    }
}
