using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections;
using HuggingFace.API

public class VoiceControlManager : MonoBehaviour {
    [SerializeField] private Button startButton;
    [SerializeField] private Button stopButton;
    public bool isActive = true;

    private AudioClip clip;
    private bool recording = false;

    private void Start() {
        startButton.onClick.AddListener(StartRecording);
        stopButton.onClick.AddListener(StopRecording);
        stopButton.interactable = false;
    }

    private void Update() {
         if (isActive) {
            if (recording && Microphone.GetPosition(null) >= clip.samples) {
                StopRecording();
            }
        }
    }

    private void StartRecording() {
        startButton.interactable = false;
        stopButton.interactable = true;
        clip = Microphone.Start(null, false, 10, 44100);
        recording = true;
    }

    private IEnumerator StopRecording() {
        var position = Microphone.GetPosition(null);
        Microphone.End(null);
        var samples = new float[position * clip.channels];
        clip.GetData(samples, 0);
        var bytes = EncodeAsWAV(samples, clip.frequency, clip.channels);
        recording = false;
        yield return StartCoroutine(SendRecording(bytes));
    }


    private byte[] EncodeAsWAV(float[] samples, int frequency, int channels) {
            using (var memoryStream = new MemoryStream(44 + samples.Length * 2)) {
                using (var writer = new BinaryWriter(memoryStream)) {
                    writer.Write("RIFF".ToCharArray());
                    writer.Write(36 + samples.Length * 2);
                    writer.Write("WAVE".ToCharArray());
                    writer.Write("fmt ".ToCharArray());
                    writer.Write(16);
                    writer.Write((ushort)1);
                    writer.Write((ushort)channels);
                    writer.Write(frequency);
                    writer.Write(frequency * channels * 2);
                    writer.Write((ushort)(channels * 2));
                    writer.Write((ushort)16);
                    writer.Write("data".ToCharArray());
                    writer.Write(samples.Length * 2);

                    foreach (var sample in samples) {
                        writer.Write((short)(sample * short.MaxValue));
                    }
                }
                return memoryStream.ToArray();
            }
        }

    private void SendRecording(byte[] bytes) {
        HuggingFaceAPI.AutomaticSpeechRecognition(bytes, response => {

        });
    }


    private void ProcessVoiceCommand(string transcription) {
    transcription = transcription.ToLower();

    if (transcription.Contains("move up")) {
        MoveUnit(Vector3Int.up);
    } else if (transcription.Contains("move down")) {
        MoveUnit(Vector3Int.down);
    } else if (transcription.Contains("move right")) {
        MoveUnit(Vector3Int.right);
    } else if (transcription.Contains("move left")) {
        MoveUnit(Vector3Int.left);
    }
}

private void MoveUnit(Vector3Int direction) {
   
}

}
