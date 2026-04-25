using UnityEngine;

public class DataShow : MonoBehaviour
{
    [SerializeField] private ObjectDataScriptable data;

    public ObjectDataScriptable getData()
    {
        return data;
    }
}
