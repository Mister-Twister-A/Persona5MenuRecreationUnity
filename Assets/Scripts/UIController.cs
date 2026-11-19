using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIController : MonoBehaviour
{
    [SerializeField] private List<GameObject> Buttons = new List<GameObject>();
    [SerializeField] private List<string> buttonDesc = new List<string>();
    [SerializeField] private GameObject curButton;
    [SerializeField] private int curButtonID = 0;
    private RectTransform curButtonRect;
    [SerializeField] private GameObject redSelector;
    [SerializeField] private GameObject blueSelector;
    [SerializeField] private TMP_Text buttonNum;
    [SerializeField] private TMP_Text buttonDesc_txt;

    void Awake()
    {
        ChangeCurButton(0);
    }

    private void ChangeCurButton(int id)
    {
        curButton = Buttons[id];
        curButtonRect = curButton.GetComponent<RectTransform>();
        buttonNum.text = $"{id + 1}";
        buttonDesc_txt.text = buttonDesc[id];
    }

    void Update()
    {
        Encapsulate(redSelector, curButtonRect,0f);
        Encapsulate(blueSelector, curButtonRect,0f);
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            curButtonID+=1;
            if (curButtonID == Buttons.Count) curButtonID = 0;
            ChangeCurButton(curButtonID);
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            curButtonID-=1;
            if (curButtonID == -1) curButtonID = Buttons.Count-1;
            ChangeCurButton(curButtonID);
        }
    }

    public void Encapsulate(GameObject selector, RectTransform button, float padding)
    {
        Transform sel = selector.transform;
        var mover = selector.GetComponent<UIPolygonMove>();
        if (mover != null) padding += mover.randomOffestNum;

        RectTransform rt = curButtonRect;
        Vector3[] corners = new Vector3[4];
        rt.GetWorldCorners(corners);
        
        for (int i = 0; i < sel.childCount; i += 1)
        {
            RectTransform corner = sel.GetChild(i).GetComponent<RectTransform>();
            Vector3 newpos = corners[i];
            newpos.x += Random.Range(-padding, padding);
            newpos.y += Random.Range(-padding, padding);
            corner.position = newpos;
            selector.GetComponent<UIPolygonMove>().Rebase();
        }


    }

}
