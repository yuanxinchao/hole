using UnityEngine;
using UnityEngine.UI;

public class HollowMask: MonoBehaviour
{
    public Graphic Mask;
//    private ClickArea _clickArea;//点击区域的屏蔽
    private RectTransform _needHollowRect;//需要挖空的长方形

    public Material _roundMaterial;
    public Material _rectMaterial;
    private bool _isRound;
    private bool _needUpdateArea = false;


    public RectTransform HoleRectTransform;


    public void Update()
    {
        SetHollowRectTransform(HoleRectTransform, HoleRectTransform,true);
    }
    public void SetHollowRectTransform(RectTransform hollowRectTransform, RectTransform clickRect = null, bool isRound = false)
    {
        _needHollowRect = hollowRectTransform;
        _isRound = isRound;
        //不设置点击区域时 使用挖孔区域
        if(clickRect == null)
            clickRect = _needHollowRect;

        if (_needHollowRect == null)
        {
            _needUpdateArea = false;
            RemoveMaterial();
        }
        else
        {
            _needUpdateArea = true;
            if (_isRound)
            {
                //圆
                Mask.material = _roundMaterial;
                Mask.SetMaterialDirty();
            }
            else
            {
                Mask.material = _rectMaterial;
                Mask.SetMaterialDirty();
            }
            UpdateHollowArea();
        }
    }

    public void UpdateHollowArea()
    {
        if(!_needUpdateArea)
            return;
        if (_isRound)
            UpdateRoundMask();
        else
            UpdateRectMask();
    }

    private void UpdateRoundMask()
    {
        var radius = _needHollowRect.rect.width/3*2 * _needHollowRect.lossyScale.x;

        var mat = Mask.materialForRendering;
        UpdateClipRound(mat, _needHollowRect.position, radius);
    }
    private void RemoveMaterial()
    {
        Mask.material = null;
        Mask.SetMaterialDirty();
    }

    private void UpdateRectMask()
    {
        float expandParam = 50;
        float softRange = 50f;

        var localPos = Mask.transform.InverseTransformPoint(_needHollowRect.position);
        var width = _needHollowRect.rect.width * _needHollowRect.localScale.x + expandParam;
        var hight = _needHollowRect.rect.height * _needHollowRect.localScale.y + expandParam;
        float leftX = localPos.x - _needHollowRect.pivot.x * width;
        float rightX = localPos.x + (1- _needHollowRect.pivot.x) * width;
        float bottomY = localPos.y - _needHollowRect.pivot.y * hight;
        float topY = localPos.y + (1- _needHollowRect.pivot.y) * hight;
        Vector4 v = new Vector4(leftX, bottomY, rightX, topY);
        //方
        var mat = Mask.materialForRendering;
        UpdateClipRect(mat, v, softRange, softRange);
    }
    private void UpdateClipRound(Material mat, Vector3 worldPos, float radius)
    {
#if UNITY_EDITOR
        //避免非运行时修改材质球
        if (!Application.isPlaying)
            return; 
#endif
        mat.SetVector("_HollowPos", worldPos);
        mat.SetFloat("_HollowRadius", radius);
    }
    private void UpdateClipRect(Material mat, Vector4 rectHollow, float softX, float softY)
    {
#if UNITY_EDITOR
        //避免非运行时修改材质球
        if (!Application.isPlaying)
            return; 
#endif
        mat.SetVector("_RectHollow", rectHollow);
        mat.SetFloat("_SoftnessX", softX);
        mat.SetFloat("_SoftnessY", softY);
    }
}
