using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class ScrollSimulator : MonoBehaviour,
    IPointerDownHandler,
    IDragHandler,
    IPointerUpHandler
{
    public static ScrollSimulator Instance;


    [Header("Panel Setup")]
    [SerializeField] private RectTransform panelContainer;
    //[SerializeField] private List<GameObject> panelPrefabs = new();
    [SerializeField] private GameObject panelPrefab; 

    [Header("Scrolling")]
    [SerializeField] private float panelHeight = 1024f;
    [SerializeField] private float snapSpeed = 12f;
    [SerializeField] private float dragSensitivity = 1f;

    private readonly List<RectTransform> activePanels = new();

    public int currentSnapIndex = 0;

    // Tracks the currently snapped-to panel and its source Node.
    private RectTransform currentSnappedPanel;
    private Node previousSnappedNode;

    public List<Vector2> snapPositions = new List<Vector2>();


    private RectTransform container;
    private Vector2 lastPointerPosition;

    private bool isDragging;
    private bool isSnapping;

    private float snapTargetY;
    private float currentVelocity;

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


        container = panelContainer;

        if (container == null)
        {
            Debug.LogError("Panel Container is not assigned.", this);
            enabled = false;
            return;
        }

        if (panelPrefab == null)
        {
            Debug.LogWarning("No panel prefab assigned.", this);
            enabled = false;
            return;
        }

    }

    private void Start()
    {
        CreatePanels();

    }

    public void RestartEverything()
    {
        // Stop any active interactions/snapping.
        isDragging = false;
        isSnapping = false;
        currentVelocity = 0f;

        // Disable hover on previous node if present.
        if (previousSnappedNode != null)
        {
            previousSnappedNode.SetVisible_Hover(false);
            previousSnappedNode = null;
        }

        // Destroy instantiated panel GameObjects to free memory and scene objects.
        // Use a simple foreach over the tracked RectTransforms.
        foreach (var rt in activePanels)
        {
            if (rt != null)
            {
                Destroy(rt.gameObject);
            }
        }

        // Clear tracking lists and references.
        activePanels.Clear();
        snapPositions.Clear();
        currentSnappedPanel = null;
        currentSnapIndex = -1;

        // Reset container position so newly created panels start from zero.
        if (container != null)
        {
            container.anchoredPosition = Vector2.zero;
        }

        // Recreate panels from the current node data.
        CreatePanels();
    }

    private void CreatePanels()
    {
        for (int i = 0; i < NodeMapManager.Instance.currentNode.nextNodes.Count; i++)
        {
            Node node = NodeMapManager.Instance.currentNode.nextNodes[i];

            GameObject instance = Instantiate(panelPrefab, container);
            RectTransform panel = instance.GetComponent<RectTransform>();

            if (panel == null)
            {
                Debug.LogError(
                    "Every panel prefab must have a RectTransform.");
                Destroy(instance);
                continue;
            }

            panel.anchorMin = new Vector2(0f, 1f);
            panel.anchorMax = new Vector2(1f, 1f);
            panel.pivot = new Vector2(0.5f, 0.5f);

            panel.sizeDelta = new Vector2(0f, panelHeight);
            panel.anchoredPosition = new Vector2(0f, -activePanels.Count * panelHeight);

            activePanels.Add(panel);
            snapPositions.Add(panel.position);

            Panel panelInfo = instance.GetComponent<Panel>();
            if (panelInfo != null)
            {
                panelInfo.infoText.text = $"NODE: {node.contentType}\nInformation here";
                panelInfo.titleText.text = node.title;
                // Link the panel back to its source Node so we can toggle hover.
                panelInfo.sourceNode = node;
            }
        }

        if (activePanels.Count > 0)
        {
            container.anchoredPosition = Vector2.zero;
            // Set initial snapped panel and enable its hover.
            UpdateCurrentSnappedPanel();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isDragging = true;
        isSnapping = false;
        currentVelocity = 0f;

        lastPointerPosition = eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging || activePanels.Count < 2)
            return;

        float deltaY =
            (eventData.position.y - lastPointerPosition.y)
            * dragSensitivity;

        Vector2 position = container.anchoredPosition;
        position.y += deltaY;
        container.anchoredPosition = position;

        lastPointerPosition = eventData.position;

        WrapPanels();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isDragging = false;

        if (activePanels.Count < 2)
            return;

        // Find the nearest panel position.
        float offset = container.anchoredPosition.y;

        snapTargetY = Mathf.Round(offset / panelHeight) * panelHeight;

        isSnapping = true;
    }

    private void WrapPanels()
    {
        if (activePanels.Count < 2)
            return;

        float offset = container.anchoredPosition.y;
        float totalHeight = activePanels.Count * panelHeight;

        // Scroll down: recycle the top panel to the bottom.
        while (offset <= -panelHeight)
        {
            RectTransform first = activePanels[0];

            activePanels.RemoveAt(0);
            activePanels.Add(first);

            first.anchoredPosition = new Vector2(
                first.anchoredPosition.x,
                first.anchoredPosition.y - totalHeight
            );

            offset += panelHeight;
        }

        // Scroll up: recycle the bottom panel to the top.
        while (offset >= panelHeight)
        {
            int lastIndex = activePanels.Count - 1;
            RectTransform last = activePanels[lastIndex];

            activePanels.RemoveAt(lastIndex);
            activePanels.Insert(0, last);

            last.anchoredPosition = new Vector2(
                last.anchoredPosition.x,
                last.anchoredPosition.y + totalHeight
            );

            offset -= panelHeight;
        }

        container.anchoredPosition = new Vector2(
            container.anchoredPosition.x,
            offset
        );

        // Keep current snapped panel accurate while wrapping if not actively dragging/snapping.
        if (!isDragging && !isSnapping)
        {
            UpdateCurrentSnappedPanel();
        }
    }

    private void Update()
    {
        if (!isSnapping || isDragging)
            return;

        Vector2 position = container.anchoredPosition;

        position.y = Mathf.SmoothDamp(
            position.y,
            snapTargetY,
            ref currentVelocity,
            1f / snapSpeed
        );

        container.anchoredPosition = position;

        if (Mathf.Abs(position.y - snapTargetY) < 0.1f)
        {
            position.y = snapTargetY;
            container.anchoredPosition = position;

            isSnapping = false;
            currentVelocity = 0f;

            // Update the tracked snapped panel and toggle hover states.
            UpdateCurrentSnappedPanel();
        }
    }

    // Finds which active panel is currently snapped (closest to center) and updates tracking fields.
    private void UpdateCurrentSnappedPanel()
    {
        if (activePanels.Count == 0 || container == null)
        {
            // Clear any previous hover.
            if (previousSnappedNode != null)
            {
                previousSnappedNode.SetVisible_Hover(false);
                previousSnappedNode = null;
            }

            currentSnapIndex = -1;
            currentSnappedPanel = null;
            return;
        }

        float bestDistance = float.MaxValue;
        int bestIndex = 0;

        for (int i = 0; i < activePanels.Count; i++)
        {
            // Distance from panel to the center (0) considering container offset.
            float distance = Mathf.Abs(activePanels[i].anchoredPosition.y + container.anchoredPosition.y);
            if (distance < bestDistance)
            {
                bestDistance = distance;
                bestIndex = i;
            }
        }

        currentSnapIndex = bestIndex;
        RectTransform newSnapped = activePanels[bestIndex];

        // If the snapped panel hasn't changed, nothing to do.
        if (currentSnappedPanel == newSnapped)
            return;

        // Determine the Node for the new snapped panel.
        Panel panelComp = newSnapped.GetComponent<Panel>();
        Node newNode = panelComp != null ? panelComp.sourceNode : null;

        // Disable hover on previous node (if any and different).
        if (previousSnappedNode != null && previousSnappedNode != newNode)
        {
            previousSnappedNode.SetVisible_Hover(false);
        }

        // Enable hover on the new node.
        if (newNode != null && newNode != previousSnappedNode)
        {
            newNode.SetVisible_Hover(true);
        }

        // Update trackers.
        previousSnappedNode = newNode;
        currentSnappedPanel = newSnapped;
    }
}