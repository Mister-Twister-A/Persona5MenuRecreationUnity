using System.Collections;
using UnityEngine;

public class UIPolygonMove : MonoBehaviour
{
    public int randomOffestNum = 15;
    public float duration = 0.2f;

    Vector2[] basePos;

    void OnEnable()
    {
        Rebase();
        for (int i = 0; i < transform.childCount; i++)
            StartCoroutine(UpdateChildPos((RectTransform)transform.GetChild(i), i));
    }
    public void Rebase()
    {
        basePos = new Vector2[transform.childCount];
        for (int i = 0; i < basePos.Length; i++)
            basePos[i] = ((RectTransform)transform.GetChild(i)).anchoredPosition;
    }

    IEnumerator UpdateChildPos(RectTransform child, int index)
    {
        Vector2 fromPos = basePos[index];
        Vector2 targetPos = basePos[index] + RandomOffset(randomOffestNum);
        float pastTime = 0;
        while (true)
        {
            pastTime += Time.deltaTime;
            if (pastTime >= duration)
            {
                fromPos = targetPos;
                targetPos = basePos[index] + RandomOffset(randomOffestNum);
                pastTime = 0;
            }
            child.anchoredPosition = Vector2.Lerp(fromPos, targetPos, pastTime / duration);
            yield return null;
        }
    }

    Vector2 RandomOffset(float max) =>
        new Vector2(Random.Range(-max, max), Random.Range(-max, max));
}