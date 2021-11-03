using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogPanelUI : MonoBehaviour
{
    public Text text;
    private Action _callback;

    public static DialogPanelUI instance;
    
    public void Show(string content, Action callback)
    {
        text.text = content;
        _callback = callback;
        gameObject.SetActive(true);
    }

    public void OnConfirm()
    {
        this.gameObject.SetActive(false);
        _callback?.Invoke();
    }
    
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        this.gameObject.SetActive(false);   
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
