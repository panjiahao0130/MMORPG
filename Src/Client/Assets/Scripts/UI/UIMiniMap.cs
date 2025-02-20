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
    
    private Transform playerTransform;

    // Start is called before the first frame update
    void Start()
    {
        MiniMapManager.Instance.minimap = this;
        InitMap();
        Debug.Log("map : " + User.Instance.CurrentMapData.MiniMap);
    }
    public void InitMap()
    {
        mapName.text = User.Instance.CurrentMapData.Name;
        if (minimap.overrideSprite == null)
            minimap.overrideSprite = MiniMapManager.Instance.LoadCurrentMinimap();
        this.minimap.SetNativeSize();
        this.minimap.transform.localPosition = Vector3.zero;
    }
    void Update()
    {
        if (playerTransform==null)
        {
            playerTransform = MiniMapManager.Instance.PlayerTransform;
        }

        if (minimapBoundinBox == null||playerTransform==null)
        {
            return;
        }
        float realWidth = minimapBoundinBox.bounds.size.x;
        float realHeight = minimapBoundinBox.bounds.size.z;
        float relaX = playerTransform.position.x - minimapBoundinBox.bounds.min.x;
        float relaY = playerTransform.position.z - minimapBoundinBox.bounds.min.z;
        float pivotX = relaX / realWidth;
        float pivotY = relaY / realHeight;

        this.minimap.rectTransform.pivot = new Vector2(pivotX, pivotY);
        this.minimap.rectTransform.localPosition = Vector2.zero;
        this.arrow.transform.eulerAngles = new Vector3(0, 0, -playerTransform.eulerAngles.y);
    }
    
}
