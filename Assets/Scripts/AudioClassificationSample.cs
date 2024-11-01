using System.Collections;
using TensorFlowLite;
using Unity.Mathematics;
using UnityEngine;

public sealed class AudioClassificationSample : MonoBehaviour
{
    [Header("Configs")] [SerializeField, FilePopup("*.tflite")]
    private string modelFile = string.Empty;

    [SerializeField] private TextAsset labelFile;

    [SerializeField] [Range(0.1f, 5f)] private float runEachNSec = 0.2f;


    [SerializeField] private MicrophoneBuffer.Options microphoneOptions = new();

    [Header("References")] [SerializeField]
    private RadarDetection radarDetection;

    private AudioClassification classification;
    private string[] labelNames;
    private MicrophoneBuffer mic;

    private IEnumerator Start()
    {
        labelNames = labelFile.text.Split('\n');
        classification = new AudioClassification(FileUtil.LoadFile(modelFile));

        mic = new MicrophoneBuffer();
        yield return mic.StartRecording(microphoneOptions);

        if (!mic.IsRecording)
        {
            radarDetection.UpdateConsole("System initialization failed. Retrying....");
            yield return new WaitForSeconds(5f);
            yield return Start();
        }
        else
        {
            radarDetection.UpdateConsole("System initialized...");
        }

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

    private void RetryMicStart()
    {
        mic?.Dispose();
        mic = new MicrophoneBuffer();
        StartCoroutine(mic.StartRecording(microphoneOptions));
    }

    private void Run()
    {
        if (!mic.IsRecording)
        {
            return;
        }

        mic.GetLatestSamples(classification.Input);

        var micInput = classification.Input;
        var amplitude = 0f;
        foreach (var input in micInput)
        {
            amplitude += math.abs(input);
        }

        amplitude /= micInput.Length;


        classification.Run();

        var topLabels = classification.GetTopLabels(4);


        for (var i = 0; i < topLabels.Length; i++)
        {
            if (labelNames[topLabels[i].id].Equals("Fart\r"))
            {
                radarDetection.Detection(amplitude);
            }
        }
    }
}