using UnityEngine;

public class CreateRadar : MonoBehaviour
{
    [Header("Number of vertical lines")] [SerializeField]
    private int numOfVertical = 10;

    [Header("Number of horizontal lines")] [SerializeField]
    private int numOfHorizontal = 10;

    [Header("Radar lines properties")] [SerializeField]
    private float radarLineWidth = 0.015f;

    [SerializeField] private float radarLineSpeed = 1.0f;


    [SerializeField] private GameObject linePrefab;
    private GameObject radarLine;
    private float viewportHeight;

    private float viewportWidth;

    private void Start()
    {
        if (!Camera.main)
        {
            Debug.LogError(
                "Main camera not found. Please ensure there is a camera tagged as 'MainCamera' in the scene.");
            return;
        }

        viewportWidth = Camera.main.orthographicSize * 2 * Camera.main.aspect;
        viewportHeight = Camera.main.orthographicSize * 2;
        CreateGrid();

        radarLine = Instantiate(linePrefab, new Vector3(-viewportHeight / 2, 0, 0), Quaternion.identity);
        radarLine.transform.localScale = new Vector3(radarLineWidth * 4, viewportHeight, 1);
    }

    private void Update()
    {
        radarLine.transform.position += radarLineSpeed * Time.deltaTime * Vector3.right;

        // Reset position if it goes out of bounds
        if (radarLine.transform.position.x > viewportWidth / 2)
        {
            radarLine.transform.position = new Vector3(-viewportWidth / 2, 0, 0);
        }
    }

    private void CreateGrid()
    {
        var verticalSpacing = viewportWidth / (numOfVertical + 1);
        var horizontalSpacing = viewportHeight / (numOfHorizontal + 1);

        for (var i = 0; i <= numOfVertical + 1; i++)
        {
            var position = new Vector3(-viewportWidth / 2 + i * verticalSpacing, 0, 0);
            var line = Instantiate(linePrefab, position, Quaternion.identity);
            line.transform.localScale = new Vector3(radarLineWidth, viewportHeight, 1);
        }

        for (var i = 0; i <= numOfHorizontal + 1; i++)
        {
            var position = new Vector3(0, -viewportHeight / 2 + i * horizontalSpacing, 0);
            var line = Instantiate(linePrefab, position, Quaternion.Euler(0, 0, 90));
            line.transform.localScale = new Vector3(radarLineWidth, viewportWidth, 1);
        }
    }
}