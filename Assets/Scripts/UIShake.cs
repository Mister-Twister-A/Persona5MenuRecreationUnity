using System.Collections;
using UnityEngine;

public class UIShake : MonoBehaviour
{
    public RectTransform target;
    public float duration = 0.3f;
    public float magnitude = 15f;    
    public float frequency = 40f;    

    Vector2 _origin;
    Coroutine _routine;

    void Awake()
    {
        if (target == null) target = (RectTransform)transform;
        _origin = target.anchoredPosition;
    }
    public void Shake()
    {
        if (_routine != null) StopCoroutine(_routine);
        _routine = StartCoroutine(ShakeRoutine());
    }

    IEnumerator ShakeRoutine()
    {
        float t = 0f;
        float seed = Random.value * 100f;

        while (t < duration)
        {
            float falloff = 1f - t / duration;   // fades out
            float x = (Mathf.PerlinNoise(seed, t * frequency) - 0.5f) * 2f;
            float y = (Mathf.PerlinNoise(seed + 50f, t * frequency) - 0.5f) * 2f;

            target.anchoredPosition = _origin + new Vector2(x, y) * magnitude * falloff;

            t += Time.deltaTime;
            yield return null;
        }

        target.anchoredPosition = _origin;
        _routine = null;
    }
}