using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HudCanvasBehaviour : MonoBehaviour
{
    public float desktopY = 226;
    public bool mobileInverted = false; 

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        float scaleFactor = (Screen.width < GlobalConfig.mobileBreakpoint) ? .5f : 1f;
        float posY = mobileInverted && scaleFactor < 1 ? Screen.height - (scaleFactor * desktopY) : (scaleFactor * desktopY);
        CanvasScaler canvasScaler = GetComponent<CanvasScaler>();
        transform.localScale = new Vector3(
            scaleFactor,
            scaleFactor,
            1
        );
        transform.position = new Vector3(
            transform.position.x,
            posY,
            transform.position.z
        );
    }
}
