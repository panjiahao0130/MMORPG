using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Object = UnityEngine.Object;

public class UIManager : Singleton<UIManager>
{
    class UIElement
    {
        public string UIResources;
        public bool Cache;
        public GameObject Instance;
    }
    private Dictionary<Type, UIElement> UIResources = new Dictionary<Type, UIElement>();

    public UIManager()
    {
        //this.UIResources.Add(typeof(GameObject), new UIElement() { UIResources = "UI/UIMain", Cache = true });
    }

    public T Show<T>()
    {
        Type type = typeof(T);
        if (UIResources.ContainsKey(type))
        {
            UIElement info= UIResources[type];
            if (info.Instance != null)
            {
                info.Instance.SetActive(true);
            }
            else
            {
                GameObject prefab = Resources.Load<GameObject>(info.UIResources);
                if (prefab==null)
                {
                    return default(T);
                }
                info.Instance = (GameObject)GameObject.Instantiate(prefab);
            }
            return info.Instance.GetComponent<T>();
        }
        return default(T);
    }

    public void Close(Type type)
    {
        if (UIResources.ContainsKey(type))
        {
            UIElement info = UIResources[type];
            if (info.Cache)
            {
                info.Instance.SetActive(false);
            }
            else
            {
                GameObject.Destroy(info.Instance);
                info.Instance = null;
            }
        }
    }
}
