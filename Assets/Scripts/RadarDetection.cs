using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RadarDetection : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject radarPing;
    [SerializeField] private CreateRadar createRadarReference;

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI gsText;
    [SerializeField] private TextMeshProUGUI tlText;
    [SerializeField] private TextMeshProUGUI lkfText;
    [SerializeField] private TextMeshProUGUI consoleText;

    private RadarPing selectedPing;
    private GameObject radarLine;
    private int currentThreatLevel = 0;
    private float totalAmplitude = 0f;
    private int detectionCount = 0;
    private float lastDetectionTime = 0f;
    private const float DetectionInterval = 5f; 

    private readonly List<string> threatLevels = new List<string>
    {
        "Clean",
        "Noticeable",
        "Unmistakable",
        "Severe",
        "Critical",
        "Extreme",
        "Chemical warfare",
        "Mass destruction"
    };

    private void Start()
    {
        radarLine = createRadarReference.radarLine;
        UpdateConsole("System initialized...");
    }

    private void Update()
    {
        if (Time.time - lastDetectionTime > DetectionInterval && currentThreatLevel > 0)
        {
            UpdateThreatLevel(currentThreatLevel - 1);
            lastDetectionTime = Time.time;
        }
    }

    public void Detection(float amplitude)
    {
        // Update last detection time on a new detection
        lastDetectionTime = Time.time;
        
        
        // Handle radar ping creation
        var lineHalfHeight = createRadarReference.lineHeight / 2 - 0.1f;
        var randomFactor = Random.Range(-0.5f, 0.5f);
        var yPos = Mathf.Lerp(radarLine.transform.position.y + lineHalfHeight, radarLine.transform.position.y - lineHalfHeight, amplitude * 4f + randomFactor);
        var radarPingInstance = Instantiate(radarPing, new Vector3(radarLine.transform.position.x, yPos, radarLine.transform.position.z), Quaternion.identity);
        var radarPingComponent = radarPingInstance.GetComponent<RadarPing>();

        radarPingInstance.transform.localScale *= Mathf.Max(amplitude * 4f, 0.1f);
        radarPingComponent.Initialize(this);
        lkfText.text = System.DateTime.Now.ToString("HH:mm:ss");

        // Update amplitude and detection count
        totalAmplitude += amplitude;
        detectionCount++;

        // Update the threat level
        CalculateThreatLevel();
    }
    
    private void CalculateThreatLevel()
    {
        var weightedAmplitude = totalAmplitude * 0.6f;
        var weightedCount = detectionCount * 0.4f;
        
        var newThreatLevel = Mathf.Clamp((int)((weightedAmplitude + weightedCount) / 10), 0, threatLevels.Count - 1);
        
        UpdateThreatLevel(newThreatLevel);
    }
    
    private void UpdateThreatLevel(int newThreatLevel)
    {
        if (newThreatLevel < 0)
        {
            newThreatLevel = 0;
        }
        
        else if (newThreatLevel >= threatLevels.Count)
        {
            newThreatLevel = threatLevels.Count - 1;
        }
        
        if (newThreatLevel != currentThreatLevel)
        {
            currentThreatLevel = newThreatLevel;
            tlText.text = threatLevels[currentThreatLevel];
            UpdateConsole($"Threat level updated to: {threatLevels[currentThreatLevel]}");
        }
    }

    public void SelectPing(RadarPing ping)
    {
        if (selectedPing != null)
        {
            selectedPing.Deselect();
        }
        selectedPing = ping;
        var gasComposition = selectedPing.Select();
        gsText.text = gasComposition;
    }

    public void OnPingDestroyed(RadarPing ping)
    {
        if (selectedPing == ping)
        {
            selectedPing = null;
            gsText.text = "No signature selected";
        }
    }

    private void UpdateConsole(string message)
    {
        StartCoroutine(UpdateConsoleCoroutine(message));
    }

    private IEnumerator UpdateConsoleCoroutine(string message)
    {
        consoleText.text = "";
        foreach (var letter in message)
        {
            consoleText.text += letter;
            yield return new WaitForSeconds(0.1f);
        }
    }
}
