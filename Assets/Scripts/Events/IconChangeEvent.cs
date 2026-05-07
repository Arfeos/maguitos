using UnityEngine;

public class IconChangeEvent : GameEventBase
{
    public string newIconUrl;

    public IconChangeEvent(string newIconUrl)
    {
        this.newIconUrl = newIconUrl;
    }
}
