using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResponsiveCanvasBehaviour : MonoBehaviour
{
    public int desktopTopOffset;
    public int mobileTopOffset;
    public float mobileScaleFactor = .5f;

    private bool _initted;
    private bool _isMobile;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        bool isMobile = Screen.width < GlobalConfig.mobileBreakpoint;
        if (_isMobile != isMobile || !_initted) {
            RectTransform rectTransform = GetComponent<RectTransform>();
            int newOffset = isMobile ? mobileTopOffset : desktopTopOffset;
            float scaleFactor = isMobile ? mobileScaleFactor : 1f;
            CanvasScaler canvasScaler = GetComponent<CanvasScaler>();
            Debug.Log(scaleFactor);
            rectTransform.localScale = new Vector3(
                scaleFactor,
                scaleFactor,
                1
            );
            rectTransform.anchoredPosition = new Vector3(
                rectTransform.anchoredPosition.x,
                newOffset
            );
            _isMobile = isMobile;
            _initted = true;
        }
    }
}
