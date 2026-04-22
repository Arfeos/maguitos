using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class AvatarGrid : MonoBehaviour
{
    [Header("Configuración")]
    public GameObject buttonPrefab;
    public Transform gridContent;
    public List<Sprite> allAvatars;

    private Image selectedImage;

    void Start()
    {
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
        }
    }

    void OnAvatarClicked(Image clickedImage)
    {
        selectedImage = clickedImage;
        Debug.Log("Avatar seleccionado: " + clickedImage.sprite.name);
    }
} 