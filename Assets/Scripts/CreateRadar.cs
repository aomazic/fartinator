using UnityEngine;

public class CreateRadar : MonoBehaviour
{
    [Header("Number of vertical lines")] [SerializeField]
    private int numOfVertical = 10;

    [Header("Number of horizontal lines")] [SerializeField]
    private int numOfHorizontal = 10;

    [Header("Radar lines properties")] [SerializeField]
    private float radarLineWidth = 0.015f;

    [Header("Ui references")] [SerializeField]
    private RectTransform bottomPanel;

    [SerializeField] private RectTransform topPanel;

    [SerializeField] private float radarLineSpeed = 1.0f;
    [SerializeField] private GameObject linePrefab;
    private float lineHeight;

    private GameObject radarLine;

    private float viewportWidth;
    private float yPos;

    private void Start()
    {
        if (!Camera.main)
        {
            Debug.LogError(
                "Main camera not found. Please ensure there is a camera tagged as 'MainCamera' in the scene.");
            return;
        }

        viewportWidth = Camera.main.orthographicSize * 2 * Camera.main.aspect;

        // Calculate the world height of the bottom and top panels
        var bottomPanelHeight = bottomPanel.rect.height * bottomPanel.lossyScale.y;
        var topPanelHeight = topPanel.rect.height * topPanel.lossyScale.y;

        // Calculate the height of the radar lines
        lineHeight = Camera.main.orthographicSize * 2 - bottomPanelHeight - topPanelHeight;
        yPos = -Camera.main.orthographicSize + bottomPanelHeight + lineHeight / 2;

        CreateGrid();

        radarLine = Instantiate(linePrefab, new Vector3(-viewportWidth / 2, yPos, 0), Quaternion.identity);
        radarLine.transform.localScale = new Vector3(radarLineWidth * 4, lineHeight, 1);
    }

    private void Update()
    {
        radarLine.transform.position += radarLineSpeed * Time.deltaTime * Vector3.right;

        // Reset position if it goes out of bounds
        if (radarLine.transform.position.x > viewportWidth / 2)
        {
            radarLine.transform.position = new Vector3(-viewportWidth / 2, yPos, 0);
        }
    }

    private void CreateGrid()
    {
        var verticalSpacing = viewportWidth / (numOfVertical + 1);
        var horizontalSpacing = lineHeight / (numOfHorizontal + 1);

        for (var i = 0; i <= numOfVertical + 1; i++)
        {
            var position = new Vector3(-viewportWidth / 2 + i * verticalSpacing, yPos, 0);
            var line = Instantiate(linePrefab, position, Quaternion.identity);
            line.transform.localScale = new Vector3(radarLineWidth, lineHeight, 1);
        }

        for (var i = 0; i <= numOfHorizontal + 1; i++)
        {
            var position = new Vector3(0, yPos - lineHeight / 2 + i * horizontalSpacing, -1f);
            var line = Instantiate(linePrefab, position, Quaternion.Euler(0, 0, 90));
            line.transform.localScale = new Vector3(radarLineWidth, viewportWidth, 1);
        }
    }
}