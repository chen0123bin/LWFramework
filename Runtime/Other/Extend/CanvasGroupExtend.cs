using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CanvasGroupExtend
{
    /// <summary>
    /// 控制CanvasGroup显示隐藏
    /// </summary>
    /// <param name="canvasGroup"></param>
    /// <param name="isActive"></param>
    public static void SetActive(this CanvasGroup canvasGroup, bool isActive) {
        if (isActive)
        {
            canvasGroup.alpha = 1;
            
        }
        else {
            canvasGroup.alpha = 0;
        }
        canvasGroup.blocksRaycasts = isActive;
    }
}
