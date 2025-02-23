using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public enum WindowResult
{
    None,
    Yes,
    No
}
public abstract class UIWindow : MonoBehaviour
{
    public delegate void CloseHandler(UIWindow sender,WindowResult result);
    public event CloseHandler OnClose;
    
    public virtual Type Type => GetType();

    public void Close(WindowResult result = WindowResult.None)
    {
        UIManager.Instance.Close(this.Type);
        if (OnClose!= null)
        {
            OnClose(this, result);
        }
        OnClose = null;
    }

    public virtual void OnCloseClick()
    {
        Close();
    }

    public virtual void OnNoClick()
    {
        Close(WindowResult.No);
    }
    public virtual void OnYesClick()
    {
        Close(WindowResult.Yes);
    }

    void OnMouseDown()
    {
        Debug.Log(this.name + "Clicked");
    }
}