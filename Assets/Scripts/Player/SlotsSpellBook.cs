using NUnit.Framework;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class SlotsSpellBook : MonoBehaviour
{
    private enum GameMode
    {
        allvsall,
        knowledgerun,
        friendly
    }
    //private List<MethodInfo> spells = new List<MethodInfo>();
    //private List<FieldInfo> spellsInfo = new List<FieldInfo>();

    void Start()
    {
        //foreach (var Actualspell in typeof(SpellBase).GetMethods())
        //{
        //    spells.Add(Actualspell);
        //}
        //foreach (var campo in typeof(SpellBase).GetFields())
        //{
        //    spellsInfo.Add(campo);
        //}  
    }

    void Update()
    {
        
    }

    //private void spellWindow()
    //{
    //    GameObject Window = null;
    //    //Window = gameObject.GetComponent<Canvas>();
    //    if (Window != null)
    //    {
    //        Window.SetActive(true);
    //    }
    //}
}
