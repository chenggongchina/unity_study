using System;
using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace
{
    
    [Serializable]
    public class GameSave
    {
        [SerializeField]
        public int test;

        [SerializeField] public GameSave2 refClass;

        [SerializeField] public List<GameSave2> listObj;

        [SerializeField] public Dictionary<string, string> keyValues;
    }

    [Serializable]
    public class GameSave2
    {
        [SerializeField] public GameSave refClass;

        [SerializeField] public int code;
    }
}