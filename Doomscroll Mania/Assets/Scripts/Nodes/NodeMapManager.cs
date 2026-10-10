using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeMapManager : MonoBehaviour
{
    [SerializeField] public List<Node> nodes = new List<Node>();

    public Node currentNode;
    public static NodeMapManager Instance;


    private void Awake()
    {
        // singleton 
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    

    // Maybe this should also handle the Scroll Simulator?


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
