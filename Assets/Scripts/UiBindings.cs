using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class UiBindings : MonoBehaviour
{
    public PlayableDirector director;
    public bool restartIfPlaying = true;


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E)){
            if (director.state == PlayState.Playing && !restartIfPlaying) return;
            director.time = 0;
            director.Play();
        }
    }
}
