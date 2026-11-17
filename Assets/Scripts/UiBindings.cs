using UnityEngine;
using UnityEngine.Playables;

public class UiBindings : MonoBehaviour
{
    public PlayableDirector director;
    public bool restartIfPlaying = true;

    void Update()
    {
        if (!Input.GetKeyDown(KeyCode.E)) return;

        if (director.state == PlayState.Playing && !restartIfPlaying) return;

        director.time = 0;
        director.Play();
    }
}
