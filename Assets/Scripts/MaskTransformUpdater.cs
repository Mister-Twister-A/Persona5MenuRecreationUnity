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
        _canvasTf = target.canvas.transform; 
    }

    void LateUpdate()
    {
        if (_mat == null || mask == null) return;

        Rect r = mask.rect;
        _mat.SetMatrix(CanvasToLocal, mask.worldToLocalMatrix * _canvasTf.localToWorldMatrix);
        _mat.SetVector(Center, r.center);
        _mat.SetVector(HalfSize, r.size * 0.5f);
    }

    void OnDestroy()
    {
        if (_mat == null) return;

        if (Application.isPlaying)
            Destroy(_mat);
        else
            DestroyImmediate(_mat);
    }
}