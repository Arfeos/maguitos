using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class AvatarGrid : MonoBehaviour
{
    [Header("Configuración")]
    public GameObject buttonPrefab;
    public Transform gridContent;
    public List<Sprite> allAvatars;
    IEventService eventService;
    private Image selectedImage;

    void Start()
    {
        eventService = AppContainer.Get<IEventService>();
        GenerateGrid();
    }

    void GenerateGrid()
    {
        foreach (Sprite avatar in allAvatars)
        {
            GameObject newButton = Instantiate(buttonPrefab, gridContent);
            Image btnImg = newButton.GetComponent<Image>();
            btnImg.sprite = avatar;

            Button btn = newButton.GetComponent<Button>();
            btn.onClick.AddListener(() => OnAvatarClicked(btnImg));
        }
    }

    void OnAvatarClicked(Image clickedImage)
    {
        selectedImage = clickedImage;
        eventService.Publish(new IconChangeEvent("Icons/" + clickedImage.sprite.name));
        Debug.Log("Avatar seleccionado: " + clickedImage.sprite.name);
    }
} 