using UnityEngine;

public class CreateRadar : MonoBehaviour
{
    [Header("Number of vertical lines")] [SerializeField]
    private int numOfVertical = 10;

    [Header("Number of horizontal lines")] [SerializeField]
    private int numOfHorizontal = 10;

    [SerializeField] private GameObject linePrefab;

    private void Start()
    {
        if (!Camera.main)
        {
            Debug.LogError(
                "Main camera not found. Please ensure there is a camera tagged as 'MainCamera' in the scene.");
            return;
        }

        var viewportWidth = Camera.main.orthographicSize * 2 * Camera.main.aspect;
        var viewportHeight = Camera.main.orthographicSize * 2;

        var verticalSpacing = viewportWidth / (numOfVertical + 1);
        var horizontalSpacing = viewportHeight / (numOfHorizontal + 1);

        for (var i = 0; i <= numOfVertical + 1; i++)
        {
            var position = new Vector3(-viewportWidth / 2 + i * verticalSpacing, 0, 0);
            var line = Instantiate(linePrefab, position, Quaternion.identity);
            line.transform.localScale = new Vector3(0.015f, viewportHeight, 1);
        }

        for (var i = 0; i <= numOfHorizontal + 1; i++)
        {
            var position = new Vector3(0, -viewportHeight / 2 + i * horizontalSpacing, 0);
            var line = Instantiate(linePrefab, position, Quaternion.Euler(0, 0, 90));
            line.transform.localScale = new Vector3(0.015f, viewportWidth, 1);
        }
    }
}