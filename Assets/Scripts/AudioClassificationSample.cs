using System.Collections;
using System.Text;
using TensorFlowLite;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public sealed class AudioClassificationSample : MonoBehaviour
{
    [Header("Configs")]
    [SerializeField, FilePopup("*.tflite")]
    private string modelFile = string.Empty;

    [SerializeField]
    private TextAsset labelFile;

    [SerializeField]
    [Range(0.1f, 5f)]
    private float runEachNSec = 0.2f;
    

    [SerializeField]
    private MicrophoneBuffer.Options microphoneOptions = new();

    [Header("References")]
    [SerializeField] private RadarDetection radarDetection;

    private AudioClassification classification;
    private MicrophoneBuffer mic;
    private string[] labelNames;

    private IEnumerator Start()
    {
        labelNames = labelFile.text.Split('\n');
        classification = new AudioClassification(FileUtil.LoadFile(modelFile));

        mic = new MicrophoneBuffer();
        yield return mic.StartRecording(microphoneOptions);

        while (Application.isPlaying)
        {
            yield return new WaitForSeconds(runEachNSec);
            Run();
        }
    }

    private void OnDestroy()
    {
        classification?.Dispose();
        mic?.Dispose();
    }

    private void Run()
    {
        if (!mic.IsRecording) return;
        mic.GetLatestSamples(classification.Input);
        
        var micInput = classification.Input;
        var amplitude = 0f;
        foreach (var input in micInput)
        {
            amplitude += math.abs(input);
        }
        amplitude /= micInput.Length;
        
        
        classification.Run();

        var topLabels = classification.GetTopLabels(5);
        
        
        
        
        for (var i = 0; i < topLabels.Length; i++)
        {
            if (labelNames[topLabels[i].id].Equals("Fart\r"))
            {
                radarDetection.Detection(amplitude);
            }
        }
        
    }

}
