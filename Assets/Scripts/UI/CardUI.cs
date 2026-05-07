using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardUI : MonoBehaviour
{
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private Image icon;
    private String guid;
    UserProfile profile;
    IProfileService _profileService;
    ISceneService _sceneService;
    public void Setup(UserProfile profile)
    {
        nameText.text = profile.name;
        this.guid = profile.guid;
        this.profile = profile;
        _profileService= AppContainer.Get<IProfileService>();
        _sceneService= AppContainer.Get<ISceneService>();
        Debug.Log("Cargando imagen desde URL: " + profile.urlImage);
        icon.sprite = Resources.Load<Sprite>(profile.urlImage);

    }
    public void OnClick()
    {
        _profileService.SelectProfile(profile);

        Debug.Log("Perfil seleccionado: " + _profileService.getSelectedProfile().name+" GUID:"+_profileService.getSelectedProfile().guid);

        
        _sceneService.LoadScene(SceneNames.Main_menu);
    }
}
