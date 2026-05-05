using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class dropDown : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown dropdown;

    void Update()
    {
        //// Solo si este dropdown está seleccionado
        //if (EventSystem.current.currentSelectedGameObject == dropdown.gameObject)
        //{
        //    if (UnityEngine.Input.GetKeyDown(KeyCode.Return) || UnityEngine.Input.GetKeyDown(KeyCode.KeypadEnter))
        //    {
        //        if (dropdown.IsExpanded)
        //            dropdown.Hide();
        //        else
        //            dropdown.Show();
        //    }
        //}
    }
}
