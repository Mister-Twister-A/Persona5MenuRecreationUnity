using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
public class MaskTransformUpdater : MonoBehaviour
{
    public Graphic target;  
    public RectTransform mask;  

    static readonly int CanvasToLocal = Shader.PropertyToID("_MaskCanvasToLocal");
    static readonly int Center = Shader.PropertyToID("_MaskCenter");
    static readonly int HalfSize = Shader.PropertyToID("_MaskHalfSize");

    Material _mat;
    Transform _canvasTf;

    void Awake()
    {
        _mat = new Material(target.material);
        target.material = _mat;
        _canvasTf = target.canvas.rootCanvas.transform;
    }

    void LateUpdate()
    {
        if (_mat == null || mask == null) return;

        Matrix4x4 m = mask.worldToLocalMatrix * _canvasTf.localToWorldMatrix;

        Rect r = mask.rect;
        _mat.SetMatrix(CanvasToLocal, m);
        _mat.SetVector(Center, r.center);
        _mat.SetVector(HalfSize, r.size * 0.5f);
    }

    void OnDestroy() { if (_mat != null) Destroy(_mat); }
}