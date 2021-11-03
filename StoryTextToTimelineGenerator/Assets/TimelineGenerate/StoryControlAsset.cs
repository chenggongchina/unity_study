using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class StoryControlAsset : PlayableAsset
{
    public string Content;
    public Action OnStart;
    public Action OnFinished;

    public string line;

    public override Playable CreatePlayable (PlayableGraph graph, GameObject owner)
    {
        var template = new StroyPlayable();
        template.Content = Content;
        template.OnStart = OnStart;
        template.OnFinished = OnFinished;
        var playable = ScriptPlayable<StroyPlayable>.Create(graph, template);
        return playable;
    }
}