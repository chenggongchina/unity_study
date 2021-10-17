using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class BehaviorTestRoot : MonoBehaviour
{
    public readonly string[] behaviors = new string[] {"MoveTowards", "RotateTowards", "Escape"};
    
    public int behaviorType;
    
    public GameObject player;
    public Dropdown dropdown;
    
    private NavMeshAgent agent;
    
    void Start()
    {
        agent = player.GetComponent<NavMeshAgent>();
        dropdown.options.Clear();
        dropdown.AddOptions(behaviors.ToList());
    }

    public void OnDropdownSelect(int index)
    {
        behaviorType = index;
    }

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            //从摄像机发出到点击坐标的射线
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;
            if(Physics.Raycast(ray,out hitInfo)){
                //划出射线，只有在scene视图中才能看到
                Debug.DrawLine(ray.origin, hitInfo.point, Color.red);

                agent.destination = hitInfo.point;
            }
        }
    }
}
