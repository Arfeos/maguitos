using UnityEngine;
[System.Serializable]
public class Settings
{
   public Languages language;
    public int axisXDirection;
    public int axisYDirection;
    public float musicVolume;
    public float MasterVolume;
    public float sensibility;
    public Settings()
    {
        language = Languages.English;
        axisXDirection = 1;
        axisYDirection = 1;
        musicVolume = 1f;
        MasterVolume = 1f;
        sensibility = 1f;
    }
    public Settings(Languages language, int axisXDirection, int axisYDirection, float musicVolume, float MasterVolume, float sensibility)
    {
        this.language = language;
        this.axisXDirection = axisXDirection;
        this.axisYDirection = axisYDirection;
        this.musicVolume = musicVolume;
        this.MasterVolume = MasterVolume;
        this.sensibility = sensibility;
    }
}
