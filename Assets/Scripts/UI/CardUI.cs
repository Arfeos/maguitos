using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardUI : MonoBehaviour
{
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private Image icon;
    private String guid;
    public void Setup(string name, string image, string guid)
    {
        nameText.text = name;
        this.guid = guid;
        //icon.sprite = ;
        // si luego cargas sprite:
        // icon.sprite = data.sprite;
    }
}
