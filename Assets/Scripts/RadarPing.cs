using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class RadarPing : MonoBehaviour
{
    private Transform selectedBrackets;
    private RadarDetection radarDetection;
    private string gasComposition;
    private Collider2D pingCollider;
    
    private void Start()
    {
        
        selectedBrackets = transform.Find("Selected");
        selectedBrackets.gameObject.SetActive(false);
        
        var nitrogen = Random.Range(20, 90);
        var hydrogen = Random.Range(0, 50);
        var carbonDioxide = Random.Range(10, 30);
        var methane = Random.Range(0, 10);
        var oxygen = Random.Range(0, 1);
        
        pingCollider = GetComponent<Collider2D>();
        pingCollider.enabled = false; 
        StartCoroutine(EnableColliderAfterDelay(1f)); 
        
        gasComposition = $"N2: {nitrogen}% H2: {hydrogen}% CO2: {carbonDioxide}%\nCH4: {methane}% O2: {oxygen}%";
    }
    private IEnumerator EnableColliderAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        pingCollider.enabled = true;
    }
    
    public void Initialize(RadarDetection detection)
    {
        radarDetection = detection;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("RadarLine"))
        {
            radarDetection.OnPingDestroyed(this);
            Destroy(gameObject);
        }
    }
    
    private void OnMouseDown()
    {
        radarDetection.SelectPing(this);
    }

    public string Select()
    {
        selectedBrackets.gameObject.SetActive(true);
        return gasComposition;
    }
   
    public void Deselect()
    {
        selectedBrackets.gameObject.SetActive(false);
    }
}