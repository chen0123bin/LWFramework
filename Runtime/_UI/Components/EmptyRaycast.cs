using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 用于使空白的UI可以进行射线检测的组件
/// </summary>
public class EmptyRaycast : MaskableGraphic
{
    public void EnableRayCast(bool enable)
    {
        this.raycastTarget = enable;
    }
    protected EmptyRaycast()
    {
        useLegacyMeshGeneration = false;
    }

    protected override void OnPopulateMesh(VertexHelper toFill)
    {
        toFill.Clear();
    }
}
