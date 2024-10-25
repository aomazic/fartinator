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
    [Range(1, 10)]
    private int showTopKLabels = 3;

    [SerializeField]
    private MicrophoneBuffer.Options microphoneOptions = new();

    [Header("UI")]
    [SerializeField]
    private Text resultText = null;


    private AudioClassification classification;
    private MicrophoneBuffer mic;
    private string[] labelNames;
    private readonly Vector3[] rtCorners = new Vector3[4];
    private readonly StringBuilder sb = new();

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
        classification.Run();

        sb.Clear();
        sb.AppendLine($"Top {showTopKLabels}:");
        var labels = classification.GetTopLabels(showTopKLabels);
        for (int i = 0; i < labels.Length; i++)
        {
            sb.AppendLine($"{labelNames[labels[i].id]}: {labels[i].score * 100f:F1}%");
        }
        resultText.text = sb.ToString();
    }

}
