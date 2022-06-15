using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResponsiveCanvasBehaviour : MonoBehaviour
{
    public int desktopTopOffset;
    public int mobileTopOffset;
    public float mobileScaleFactor = .5f;

    private int _currentTopOffset;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        int newOffset = (Screen.width < GlobalConfig.mobileBreakpoint) ? mobileTopOffset : desktopTopOffset;
        float scaleFactor = (_currentTopOffset == mobileTopOffset) ? mobileScaleFactor : 1f;
        if (_currentTopOffset != newOffset || rectTransform.localScale.x != scaleFactor) {
            _currentTopOffset = newOffset;
            CanvasScaler canvasScaler = GetComponent<CanvasScaler>();
            rectTransform.localScale = new Vector3(
                scaleFactor,
                scaleFactor,
                1
            );
            rectTransform.anchoredPosition = new Vector3(
                rectTransform.anchoredPosition.x,
                _currentTopOffset
            );
        }
    }
}
