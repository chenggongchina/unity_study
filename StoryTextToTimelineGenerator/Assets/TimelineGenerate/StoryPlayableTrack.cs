using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;


[TrackClipType(typeof(StoryControlAsset))]
public class StoryControlTrack : TrackAsset {}



public class StroyPlayable : PlayableBehaviour
{
    public string Content;
    public Action OnStart;
    public Action OnFinished;
    
    private bool isEnter = false;
    private bool isLeave = false;
    private PlayableGraph graph;
    private bool pass = false;
    private const float FRAME_DELTA_TIME = 1f / 30;
    
    public override void OnPlayableCreate(Playable playable)
    {
        graph = playable.GetGraph();
    }
    
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        if (!isEnter)
        {
            isEnter = true;
            Enter();
        }

        if (!isLeave)
        {
            if (playable.GetTime() >= (playable.GetDuration() - FRAME_DELTA_TIME))
            {
                isLeave = true;
                Leave();
            }    
        }
        

        /*var time = graph.GetRootPlayable(0).GetTime();
        var time2 = playable.GetTime();
        Debug.Log(time2);
        Debug.Log(time - time2);*/
        //graph.GetRootPlayable(0).SetTime(graph.GetRootPlayable(0).GetTime() + thisPlayable.GetDuration());
    }

    void Enter()
    {
        //Debug.Log("enter called");
        OnStart?.Invoke();

        if (string.IsNullOrEmpty(Content))
        {
            Next();
        }
        else
        {
            DialogPanelUI.instance.Show(Content, Next);    
        }
    }

    void Next()
    {
        pass = true;
        Resume();
    }

    void Leave()
    {
        //Debug.Log("leave called");
        if (!pass)
        {
            Pause();
        }
        OnFinished?.Invoke();
    }


    void Reset()
    {
        pass = false; //reset pass
        isEnter = false;
        isLeave = false;
    }


    void Pause()
    {
        graph.GetRootPlayable(0).SetSpeed(0);
    }

    void Resume()
    {
        if (graph.GetRootPlayable(0).GetSpeed() == 0)
        {
            graph.GetRootPlayable(0).SetSpeed(1);    
        }
    }
}


