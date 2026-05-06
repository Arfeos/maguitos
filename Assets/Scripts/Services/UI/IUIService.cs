using UnityEngine;
using UnityEngine.UI;
public interface IUIService
{
    public void RegisterFirstButton(Button[] foundButtons);
     Button FirstButton { get;  }
    public void changeLanguage(Languages language);

}
