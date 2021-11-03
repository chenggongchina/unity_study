using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TimelineGeneratorDemo))]
public class TimelineGeneratorDemoEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        
        if (GUILayout.Button("Generate"))
        {
            TimelineGeneratorDemo target = this.target as TimelineGeneratorDemo;
            
            var asset = target.GenerateTimelineAsset(target.fileContent.text);
            AssetDatabase.CreateAsset(asset, "Assets/TimelineGenerate/generate.asset");
        }
    }
}
