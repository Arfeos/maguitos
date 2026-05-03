using UnityEngine;
using UnityEngine.UI;
public interface IUIService
{
    public void RegisterFirstButton(Selectable[] foundButtons);
     Selectable FirstButton { get;  }
    public void changeLanguage(Languages language);

}
