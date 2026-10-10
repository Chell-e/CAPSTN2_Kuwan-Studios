using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public enum NodeType
{
    Event,
    Battle,
    Final
}

public class Node : MonoBehaviour
{
    public List<Node> nextNodes; // available nodes to travel to from here

    public NodeType contentType; // this node's content type; information to be loaded in Scrolling Simulator

    [SerializeField] public Image typeIcon;
    [SerializeField] public Image hoverIcon;
    [SerializeField] public Image statusIcon;

    // FLAVOR
    [SerializeField] public string title;



    public void SetVisible_Hover(bool isEnabled)
    {
        if (isEnabled)
            hoverIcon.enabled = true;
        else
            hoverIcon.enabled = false;
    }

    public void SetVisible_Status(bool isEnabled)
    {
        if (isEnabled)
            statusIcon.enabled = true;
        else
            statusIcon.enabled = false;
    }



}
