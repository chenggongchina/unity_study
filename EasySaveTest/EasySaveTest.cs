using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.Assertions;

public class EasySaveTest : MonoBehaviour
{
    public Transform obj;
    
    public void Save()
    {
        GameSave save = new GameSave();
        save.test = 100023;

        GameSave2 save2 = new GameSave2();
        save2.refClass = save;
        save.refClass = save2;

        save.listObj = new List<GameSave2>();
        for (int i = 0; i < 10; ++i)
        {
            save.listObj.Add(new GameSave2() {code = i});
        }

        save.keyValues = new Dictionary<string, string>();
        save.keyValues["t"] = "test";
        save.keyValues["1"] = "test2";
        
        ES3.Save("test", save, "save1.sav");
        ES3.Save("testTransform", obj);
    }

    public void Load()
    {
        var save = ES3.Load<GameSave>("test", "save1.sav");
        if (save != null)
        {
            Debug.Log(save.test);

            for (int i = 0; i < save.listObj.Count; ++i)
            {
                Debug.Log(save.listObj[i].code);
            }
            
            Debug.Log("count = " + save.keyValues.Count);
        }
        
        ES3.LoadInto("testTransform", obj);
    }
}
