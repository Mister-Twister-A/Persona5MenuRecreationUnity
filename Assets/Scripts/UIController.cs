using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;

public class UIController : MonoBehaviour
{
    public enum Screen {Main, Skill, Item}
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

    [Header("Item screen")]

    [SerializeField] private List<GameObject> Items = new List<GameObject>();
    private RectTransform curItemRect;
    [SerializeField] private int curItemID = 0;

    [Header("Playable directors")]
    public PlayableDirector director_skill;
    public PlayableDirector director_item;
    public bool restartIfPlaying = true;

    [SerializeField] private Screen curScreen = Screen.Main;

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

    private void ChangeCurItem(int id)
    {
  
        curItemRect = Items[id].GetComponent<RectTransform>();
        Encapsulate(redSelector, curItemRect,0f,true);
        //Encapsulate(blueSelector, curButtonRect,0f,true);
        //buttonNum.text = $"{id + 1}";
        buttonDesc_txt.text = "What item would you like to Use?";
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
        if (curScreen == Screen.Main) UpdateMain();
        if (curScreen == Screen.Skill) UpdateSkill();
        if(curScreen == Screen.Item) UpdateItem();

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
                curScreen = Screen.Skill;
                //Debug.Log(director_skill.state);
                if (director_skill.state == PlayState.Playing && !restartIfPlaying) return;
                director_skill.time = 0;
                director_skill.Play();
            }

            if (curButtonID == 1)
            {
                curScreen = Screen.Item;
                if (director_item.state == PlayState.Playing && !restartIfPlaying) return;
                director_item.time = 0;
                director_item.Play();
                StartCoroutine(WaitForDirectorEnd(director_item, 0.2f));
                //director_item.stopped -= StartOnlyRedSelector;
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
    IEnumerator WaitForDirectorEnd(PlayableDirector d, float t)
    {
        yield return null; 
        while (d.time < d.duration - t) yield return null;
        ChangeCurItem(0);
    }

    void UpdateItem()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            curItemID+=1;
            if (curItemID == Items.Count) curItemID = 0;
            ChangeCurItem(curItemID);
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            curItemID-=1;
            if (curItemID == -1) curItemID =  Items.Count - 1;
            ChangeCurItem(curItemID);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            
            StartCoroutine(Reverse(director_item));
            
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
        ChangeCurButton(curButtonID);
        curScreen = Screen.Main;
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
