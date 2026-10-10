using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Panel : MonoBehaviour
{
    [SerializeField] public TMP_Text titleText;
    [SerializeField] public TMP_Text infoText;

    public Node sourceNode;



    public void CrossNode()
    {
        this.sourceNode.SetVisible_Status(true);

        NodeMapManager.Instance.currentNode = sourceNode;


        ScrollSimulator.Instance.RestartEverything();
    }

    
}
