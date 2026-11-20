using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;

public static class ApllyTimelineEnd
{
    [MenuItem("Tools/Timeline/Apply Last Frame To Scene")]
    static void Apply()
    {
        var director = Object.FindFirstObjectByType<PlayableDirector>(); // FindObjectOfType on older Unity
        if (director == null || director.playableAsset == null) return;

        var targets = new List<Object>();
        foreach (var output in director.playableAsset.outputs)
        {
            var binding = director.GetGenericBinding(output.sourceObject);
            if (binding is Animator a)
                targets.AddRange(a.GetComponentsInChildren<Transform>(true));
            else if (binding is GameObject go)
                targets.AddRange(go.GetComponentsInChildren<Transform>(true));
            else if (binding is Component c)
            {
                targets.Add(c);
                targets.Add(c.transform);
            }
        }

        Undo.RecordObjects(targets.ToArray(), "Apply Timeline Last Frame");

        director.time = director.duration;
        director.Evaluate();

        foreach (var t in targets) EditorUtility.SetDirty(t);
    }
}