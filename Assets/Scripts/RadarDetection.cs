using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class RadarDetection : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject radarPing;
    [SerializeField] private CreateRadar createRadarReference;

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI gsText;
    [SerializeField] private TextMeshProUGUI tlText;
    [SerializeField] private TextMeshProUGUI lkfText;
    
    private RadarPing selectedPing;
    private GameObject radarLine;
    
    private void Start()
    {
        radarLine = createRadarReference.radarLine;
    }

    public void Detection(float amplitude)
    {
        var lineHalfHeight = createRadarReference.lineHeight / 2 - 0.1f;
        var randomFactor = Random.Range(-0.5f, 0.5f);
        var yPos = Mathf.Lerp(radarLine.transform.position.y + lineHalfHeight, radarLine.transform.position.y - lineHalfHeight, amplitude * 4f + randomFactor);
        var radarPingInstance = Instantiate(radarPing, new Vector3(radarLine.transform.position.x, yPos, radarLine.transform.position.z), Quaternion.identity);
        var radarPingComponent = radarPingInstance.GetComponent<RadarPing>();

        radarPingInstance.transform.localScale *= Mathf.Max(amplitude * 4f, 0.1f);
        radarPingComponent.Initialize(this);
        lkfText.text = System.DateTime.Now.ToString("HH:mm:ss");
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
}