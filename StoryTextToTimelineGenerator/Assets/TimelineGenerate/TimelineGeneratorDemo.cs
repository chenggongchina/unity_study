using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class TimelineGenerateContext
{
    //存放角色的动作轨
    public Dictionary<string, AnimationTrack> roleAnimTracks = new Dictionary<string, AnimationTrack>();
    public StoryControlTrack storyConTrolTrack;
    public TimelineAsset timelineAsset;

}

public class TimelineGeneratorDemo : MonoBehaviour
{
    [Header("动作间时间等待")] public float AnimationDialogSpan = 0.5f;
    [Header("普通对话间等待")] public float DefaultDialogSpan = 0.1f;
    [Header("动画Ease时间")] public float AnimationEaseTime = 0.1f;
    
    public TextAsset fileContent;
    public AnimationClip[] animationClips;
    public List<GameObject> npcs;
    public CinemachineVirtualCamera vcam;
    public CinemachineVirtualCamera vcamFace;
    
    private PlayableDirector pd;
    
    /// <summary>
    /// 根据台本生成用于播放的timeline
    /// </summary>
    /// <param name="content"></param>
    /// <returns></returns>
    public TimelineAsset GenerateTimelineAsset(string content)
    {
        TimelineGenerateContext context = new TimelineGenerateContext();
        context.timelineAsset = TimelineAsset.CreateInstance<TimelineAsset>();
        context.storyConTrolTrack = context.timelineAsset.CreateTrack<StoryControlTrack>(null, "storyTrack");

        //初始化NPC轨道
        foreach (var npc in npcs)
        {
            string roleName = npc.name;
            if (!context.roleAnimTracks.ContainsKey(roleName))//codereview: 反转if
            {
                context.roleAnimTracks[roleName] =
                    context.timelineAsset.CreateTrack<AnimationTrack>(null, roleName + "'s AnimationTrack");
                context.roleAnimTracks[roleName].trackOffset = TrackOffset.ApplySceneOffsets;

                if (pd != null)// codereview: pd?.SetGenericBinding
                {
                    pd.SetGenericBinding(context.roleAnimTracks[roleName], npc);    
                }
            }
        }
        
        //当前时间
        double currentTime = 0f;
        
        foreach (var t in content.Split('\n'))
        {
            string line = t.ToString();// codereview: line替换t
            
            //空行或者注释
            if (string.IsNullOrEmpty(line) || line.StartsWith("//"))
                continue;

            string cmd = "";
            string[] paras = null;
            //指令扩展
            if (line.StartsWith("["))
            {
                cmd = line.TrimStart(new char[] {'['}).Split(']')[0];
                paras = line.Split(']')[1].Split(',');

                switch (cmd)
                {
                    case "镜头反打":
                        AddCameraBack(context, ref currentTime, paras);
                        break;
                    case "镜头特写":
                        AddCameraFace(context, ref currentTime, paras);
                        break;
                    default:
                        Debug.LogError($"未定义的指令[{cmd}]");
                        break;
                }
            }
            //台本对话
            else
            {
                AddDialogCmd(context, line, ref currentTime);
            }
        }

        return context.timelineAsset;
    }

    GameObject GetNpc(string name)
    {
        return npcs.Single(npc => npc.name.Equals(name.Trim()));
    }
    
    /// <summary>
    /// 镜头反打
    /// </summary>
    /// <param name="context"></param>
    /// <param name="currentTime"></param>
    /// <param name="paras"></param>
    void AddCameraBack(TimelineGenerateContext context, ref double currentTime, string[] paras)
    {
        // code review: paras check length
        var clip = context.storyConTrolTrack.CreateClip<StoryControlAsset>();
        var story = clip.asset as StoryControlAsset;// codereview: check null
        story.Content = null;
        story.line = "镜头反打:" + paras[0].Trim();
        
        string roleName = paras[0].Trim();
        
        story.OnStart = () =>
        {
            vcamFace.gameObject.SetActive(false);
            var role = GetNpc(roleName);
            //摄像头看向角色
            vcam.Follow = role.transform;
            vcam.LookAt = GetOtherPerson(role.transform);
        };
        
        clip.start = currentTime; //比动作略微滞后
        clip.duration = 0.1f;

        currentTime += 0.2f;
    }
    
    
    /// <summary>
    /// 镜头特写
    /// </summary>
    /// <param name="context"></param>
    /// <param name="currentTime"></param>
    /// <param name="paras"></param>
    void AddCameraFace(TimelineGenerateContext context, ref double currentTime, string[] paras)
    {
        var clip = context.storyConTrolTrack.CreateClip<StoryControlAsset>();
        var story = clip.asset as StoryControlAsset;
        story.Content = null;
        story.line = "镜头特写:" + paras[0].Trim();

        string roleName = paras[0].Trim();
        
        story.OnStart = () =>
        {
            var role = GetNpc(roleName);
            vcamFace.transform.position = role.transform.position + role.transform.forward * 2 + 2 * Vector3.up;
            vcamFace.LookAt = role.transform;
            
            vcamFace.gameObject.SetActive(true);
        };
        
        clip.start = currentTime; //比动作略微滞后
        clip.duration = 0.1f;

        currentTime += 0.2f;
    }
    

    void AddDialogCmd(TimelineGenerateContext context, string line, ref double currentTime)
    {
        double addTime = 0.1f;

        var tmp = line.Trim().Split(':');// code review: tmp check length
        string roleName = tmp[0];

        //有动作描述
        if (roleName.Contains("("))
        {
            string actionName = roleName.Split('(')[1].TrimEnd(new char[] {')'});
            roleName = roleName.Split('(')[0];

            var bindRole = GetNpc(roleName);
            if (bindRole == null)
            {
                Debug.LogError($"找不到角色{roleName}!无法绑定动作");
            }
            else
            {
                AnimationTrack animTrack = context.roleAnimTracks[roleName];

                var aClip = animTrack.CreateClip<AnimationPlayableAsset>();
                var anim = aClip.asset as AnimationPlayableAsset;// code review : check null

                anim.clip = animationClips.Single(p => p.name == actionName);

                aClip.start = currentTime;
                aClip.duration = anim.clip.length;
                addTime = aClip.duration;

                aClip.easeInDuration = AnimationEaseTime;
                aClip.easeOutDuration = AnimationEaseTime;
                aClip.SetPostExtrapolationMode(TimelineClip.ClipExtrapolation.None);
                aClip.SetPreExtrapolationMode(TimelineClip.ClipExtrapolation.None);
            }
        }

        if (tmp.Length > 1)
        {
            //对话内容
            string talkContent = tmp[1];

            var clip = context.storyConTrolTrack.CreateClip<StoryControlAsset>();
            var story = clip.asset as StoryControlAsset;
            story.Content = $"{roleName} 说：{talkContent}";
            
            clip.start = currentTime + AnimationEaseTime; //比动作略微滞后
            clip.duration = addTime + AnimationEaseTime;
        }

        if (addTime > 0)
        {
            currentTime += addTime + AnimationDialogSpan; //空档    
        }
        else
        {
            currentTime += addTime + DefaultDialogSpan; //普通对话等待
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        pd = gameObject.GetComponent<PlayableDirector>();
        var asset = GenerateTimelineAsset(fileContent.text);
        pd.playableAsset = asset;
        pd.Play();
    }

    //苟且实现
    Transform GetOtherPerson(Transform person)
    {
        foreach (var npc in npcs)
        {
            if (npc.transform != person)
                return npc.transform;
        }

        return null;
    }

}
