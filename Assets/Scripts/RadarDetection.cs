using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class RadarDetection : MonoBehaviour
{
    private const float DetectionInterval = 5f;

    [Header("References")] [SerializeField]
    private GameObject radarPing;

    [SerializeField] private CreateRadar createRadarReference;

    [Header("UI References")] [SerializeField]
    private TextMeshProUGUI gsText;

    [SerializeField] private TextMeshProUGUI tlText;
    [SerializeField] private TextMeshProUGUI lkfText;
    [SerializeField] private TextMeshProUGUI consoleText;

    [Header("Sfx")] [SerializeField] private AudioClip detectionSfx;

    [SerializeField] private AudioClip alarmSfx;
    [SerializeField] private AudioClip ambientSfx;

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

    private AudioSource audioSource;
    private string consoleMessage;
    private int currentThreatLevel;
    private int detectionCount;
    private float lastDetectionTime;
    private GameObject radarLine;
    private RadarPing selectedPing;
    private float totalAmplitude;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        radarLine = createRadarReference.radarLine;

        // Loop ambient sfx
        audioSource.loop = true;
        audioSource.clip = ambientSfx;
        audioSource.Play();
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
        var yPos = Mathf.Lerp(radarLine.transform.position.y + lineHalfHeight,
            radarLine.transform.position.y - lineHalfHeight, amplitude * 7f + randomFactor);
        var radarPingInstance = Instantiate(radarPing,
            new Vector3(radarLine.transform.position.x, yPos, radarLine.transform.position.z), Quaternion.identity);
        var radarPingComponent = radarPingInstance.GetComponent<RadarPing>();

        radarPingInstance.transform.localScale *= Mathf.Max(amplitude * 4f, 0.1f);
        radarPingComponent.Initialize(this);
        lkfText.text = DateTime.Now.ToString("HH:mm:ss");

        // Update amplitude and detection count
        totalAmplitude += amplitude;
        detectionCount++;

        // Update the threat level
        CalculateThreatLevel();

        audioSource.PlayOneShot(detectionSfx);
    }

    private void CalculateThreatLevel()
    {
        var weightedAmplitude = totalAmplitude * 0.7f;
        var weightedCount = detectionCount * 0.3f;

        var newThreatLevel = Mathf.Clamp((int)((weightedAmplitude + weightedCount) / 5), 0, threatLevels.Count - 1);

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
            if (newThreatLevel > currentThreatLevel)
            {
                audioSource.PlayOneShot(alarmSfx);
            }

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

    public void UpdateConsole(string message)
    {
        consoleMessage = message;
        StopCoroutine(nameof(UpdateConsoleCoroutine));
        StartCoroutine(UpdateConsoleCoroutine());
    }

    private IEnumerator UpdateConsoleCoroutine()
    {
        consoleText.text = "";
        foreach (var letter in consoleMessage)
        {
            consoleText.text += letter;
            yield return new WaitForSeconds(0.1f);
        }
    }
}