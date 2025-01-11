using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InsManager : MonoBehaviour
{
    [SerializeField]
    private GameObject MainCamera;
    [SerializeField]
    private GameObject UIMain;
    // Start is called before the first frame update
    private void OnDestroy()
    {
        Destroy(MainCamera);
        Destroy(UIMain);
    }
}
