using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;

public class UIController : MonoBehaviour
{
    [SerializeField] private List<GameObject> Buttons = new List<GameObject>();

    [SerializeField] private List<string> buttonDesc = new List<string>();
    [SerializeField] private GameObject curButton;
    [SerializeField] private int curButtonID = 0;
    private RectTransform curButtonRect;

    // [SerializeField] private List<GameObject> Characters = new List<GameObject>();
    // [SerializeField] private List<string> charDesc = new List<string>();
    // [SerializeField] private GameObject curChar;
    // [SerializeField] private int curCharID = 0;
    // private RectTransform curCharRect;
    [SerializeField] private GameObject redSelector;
    [SerializeField] private GameObject blueSelector;
    [SerializeField] private TMP_Text buttonNum;
    [SerializeField] private TMP_Text buttonDesc_txt;

    [Header("Playable directors")]
    public PlayableDirector director_skill;
    public bool restartIfPlaying = true;

    [SerializeField] private string curScreen = "main";

    void Awake()
    {
        ChangeCurButton(0);
    }

    private void ChangeCurButton(int id)
    {
        curButton = Buttons[id];
        curButtonRect = curButton.GetComponent<RectTransform>();
        Encapsulate(redSelector, curButtonRect,0f,true);
        Encapsulate(blueSelector, curButtonRect,0f,true);
        buttonNum.text = $"{id + 1}";
        buttonDesc_txt.text = buttonDesc[id];
    }
    // private void ChangeCurChar(int id)
    // {
    //     curChar = Characters[id];
    //     curCharRect = curChar.GetComponent<RectTransform>();
    //     Encapsulate(redSelector, curCharRect,0f,true);
    //     Encapsulate(blueSelector, curCharRect,0f,true);
    //     //buttonNum.text = $"{id + 1}";
    //     buttonDesc_txt.text = "Whoose skill would you like to use?";
    // }

    void Update()
    {
        if (curScreen == "main") UpdateMain();
        if (curScreen == "skill") UpdateSkill();
    }

    void UpdateMain()
    {
        
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
        
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (curButtonID == 0)
            {
                curScreen = "skill";
                //Debug.Log(director_skill.state);
                if (director_skill.state == PlayState.Playing && !restartIfPlaying) return;
                director_skill.time = 0;
                director_skill.Play();
            }
        }
    }
    void UpdateSkill()
    {
        buttonDesc_txt.text = "Whoose skill would you like to use?"; //temporary solution
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            
            StartCoroutine(Reverse(director_skill));
            
        }

    }

    IEnumerator Reverse(PlayableDirector director)
    {
        director.timeUpdateMode = DirectorUpdateMode.Manual;   
        director.time = director.duration;
        director.Evaluate();

        while (director.time > 0)
        {
            director.time = Mathf.Max(0f, (float)director.time - Time.deltaTime);
            director.Evaluate();                  
            yield return null;
        }

        director.time = 0;
        director.Evaluate();

        director.timeUpdateMode = DirectorUpdateMode.GameTime;
        ChangeCurButton(0);
        curScreen = "main";
    }



    Vector3[] allocated_array; //idk about calling Encapsulate every frame, Ill leave it here if I need it in the future
    public void Encapsulate(GameObject selector, RectTransform button, float padding, bool need_allocation)
    {
        Transform sel = selector.transform;
        var mover = selector.GetComponent<UIPolygonMove>();
        if (mover != null) padding += mover.randomOffestNum;

        RectTransform rt = button;
        Vector3[] corners;
        if (need_allocation){ 
            corners = new Vector3[4];
            allocated_array = corners;
        }
        else corners = allocated_array;
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
