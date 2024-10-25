using UnityEngine;

public class CreateRadar : MonoBehaviour
{
    [Header("Number of vertical lines")]
    [SerializeField]
    private int numOfVertical = 10;
    
    [Header("Number of horizontal lines")]
    [SerializeField]
    private int numOfHorizontal = 10;

    [SerializeField]
    private GameObject linePrefab;

    void Start()
    {
        float viewportWidth = Camera.main.orthographicSize * 2 * Camera.main.aspect;
        float viewportHeight = Camera.main.orthographicSize * 2;

        float verticalSpacing = viewportWidth / (numOfVertical + 1);
        float horizontalSpacing = viewportHeight / (numOfHorizontal + 1);

        for (int i = 1; i <= numOfVertical; i++)
        {
            Vector3 position = new Vector3(-viewportWidth / 2 + i * verticalSpacing, 0, 0);
            GameObject line = Instantiate(linePrefab, position, Quaternion.identity);
            line.transform.localScale = new Vector3(1, viewportHeight, 1);
        }

        for (int i = 1; i <= numOfHorizontal; i++)
        {
            Vector3 position = new Vector3(0, -viewportHeight / 2 + i * horizontalSpacing, 0);
            GameObject line = Instantiate(linePrefab, position, Quaternion.Euler(0, 0, 90));
            line.transform.localScale = new Vector3(1, viewportWidth, 1);
        }
    }
}
