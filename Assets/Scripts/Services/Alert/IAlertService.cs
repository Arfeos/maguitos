using UnityEngine;

public interface IAlertService
{
    public void ShowAlertMessage(GameObject MessageBox, ObjectDataScriptable message);
    public void HideAlertMessage(GameObject MessageBox);

}
