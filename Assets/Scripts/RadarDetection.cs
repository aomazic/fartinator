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
    [SerializeField] private TextMeshProUGUI lKFText;
    
    private GameObject radarLine;
    
    private void Start()
    { 
        radarLine = createRadarReference.radarLine;
    }
    
    public void Detection(float amplitude)
    {
        var radarPingInstance = Instantiate(radarPing, radarLine.transform.position, Quaternion.identity);
        Destroy(radarPingInstance, 2.0f);
    }
    
}
