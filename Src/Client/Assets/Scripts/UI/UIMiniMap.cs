using Managers;
using Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UIMiniMap : MonoBehaviour
{
    public Text mapName;
    public Image minimap;
    public Image arrow;
    public Collider minimapBoundinBox;

    // Start is called before the first frame update
    void Start()
    {
        MiniMapManager.Instance.minimap = this;
        this.Init();
        Debug.Log("map : " + User.Instance.CurrentMapData.MiniMap);
    }
    public void Init()
    {
        UpdateMiniMap();
    }
    // Update is called once per frame
    void Update()
    {
        if (minimapBoundinBox == null)
            return;
        if (User.Instance.CurrentCharaterObject) 
            OnMapMove();
    }
    public void UpdateMiniMap()
    {
        mapName.text = User.Instance.CurrentMapData.Name;
        if (minimap.overrideSprite != null)
            //minimap.overrideSprite = MiniMapManager.Instance.LoadCurrentMinimap();
        //this.minimap.sprite = MiniMapManager.Instance.LoadCurrentMinimap();
        this.minimap.SetNativeSize();
        this.minimap.transform.localPosition = Vector3.zero;
        //this.playerMiniMap = User.Instance.CurrentCharaterObject.transform;
    }
    private void OnMapMove()
    {
        float realWidth = minimapBoundinBox.bounds.size.x;
        float realHeight = minimapBoundinBox.bounds.size.z;
        float relaX = User.Instance.CurrentCharaterObject.transform.position.x -
            minimapBoundinBox.bounds.min.x;
        float relaY = User.Instance.CurrentCharaterObject.transform.position.z -
            minimapBoundinBox.bounds.min.z;
        if (relaY == 0 || relaX == 0)
            Debug.Log("relaX: " + relaX + " relaY: " + relaY);

        float pivotX = relaX / realWidth;
        float pivotY = relaY / realHeight;

        this.minimap.rectTransform.pivot = new Vector2(pivotX, pivotY);
        this.minimap.transform.localPosition = Vector2.zero;

        this.arrow.transform.eulerAngles = new Vector3(0, 0,
            -User.Instance.CurrentCharaterObject.transform.eulerAngles.y);
    }

}
