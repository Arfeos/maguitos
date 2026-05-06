using UnityEngine;
using UnityEngine.UI;
public interface IUIService
{
    public void RegisterFirstButton(GameObject firstButton);
     Selectable FirstButton { get;  }
    public void changeLanguage(Languages language);

}
