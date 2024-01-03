using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using UnityEngine;
using Whisper;
using Whisper.Utils;
using Debug = UnityEngine.Debug;

namespace HelperScripts
{
    public class VoiceRecognition : MonoBehaviour
    {
        public WhisperManager whisper;
        public MicrophoneRecord microphoneRecord;

        private void Awake()
        {
            if (microphoneRecord != null) 
                microphoneRecord.OnRecordStop += OnRecordStop;
        }

        public void StartListening()
        {
            if (!microphoneRecord.IsRecording)
            {
                microphoneRecord.StartRecord();
                Debug.Log("Voice recognition started. Listening for commands...");
            }
        }

        public void StopListening()
        {
            if (microphoneRecord.IsRecording)
            {
                microphoneRecord.StopRecord();
                Debug.Log("Voice recognition stopped."); 
            }
        }

        private bool IsMicrophoneInitialized()
        {
            if (microphoneRecord == null || !microphoneRecord.IsRecording)
            {
                Debug.Log("microphoenRecord " + microphoneRecord);
                Debug.Log("microphoneRecord.IsRecording: " + microphoneRecord.IsRecording);
                Debug.Log("MicrophoneRecord is not initialized.");
                return false;
            }
            return true;
        }

        private async void OnRecordStop(AudioChunk recordedAudio)
        {
            var command = await TranscribeAudioAsync(recordedAudio);
            if (string.IsNullOrEmpty(command)) return;

            InterpretAndExecuteCommand(command);
        }

        private async Task<string> TranscribeAudioAsync(AudioChunk recordedAudio)
        {
            var res = await whisper.GetTextAsync(recordedAudio.Data, recordedAudio.Frequency, recordedAudio.Channels);
            if (res == null)
            {
                Debug.Log("No transcription result.");
                return null;
            }

            var command = res.Result.ToLower();
            Debug.Log($"Command processed: {command}");
            return command;
        }

        private Unit FindSelectedUnit()
        {
            foreach (Unit unit in FindObjectsOfType<Unit>())
            {
                if (unit.m_selected) return unit;
            }
            return null;
        }
        private async void InterpretAndExecuteCommand(string command)
        {
            Debug.Log($"Interpreting command: {command}");

            Unit selectedUnit = FindSelectedUnit();
            if (selectedUnit == null)
            {
                Debug.Log("No unit selected.");
                return;
            }

            string[] commands = command.Split(new string[] { "und" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var singleCommand in commands)
            {
                Debug.Log($"Processing individual command: {singleCommand.Trim()}");
                await ExecuteUnitMovement(selectedUnit, singleCommand.Trim());
            }
        }

        private async Task ExecuteUnitMovement(Unit unit, string command)
        {
            Debug.Log($"Executing movement command: {command}");
            Vector3Int moveDirection = GetMoveDirection(command);
            int steps = GetMovementSteps(command);

            if (moveDirection != Vector3Int.zero && steps > 0)
            {
                for (int i = 0; i < steps; i++)
                {
                    Vector3Int targetPosition = unit.currentPosition + moveDirection;
                    Debug.Log($"Moving unit to {targetPosition}");
                    unit.schritteGehen(targetPosition);
            
                    // Add a delay between each step
                    await Task.Delay(500); // Delay of 500 milliseconds (0.5 seconds)
                }
            }
            else
            {
                Debug.Log("Invalid move command.");
            }
        }

        private Vector3Int GetMoveDirection(string command)
        {
            if (command.Contains("nach oben")) return Vector3Int.up;
            if (command.Contains("nach unten")) return Vector3Int.down;
            if (command.Contains("nach links")) return Vector3Int.left;
            if (command.Contains("nach rechts")) return Vector3Int.right;
            return Vector3Int.zero;
        }

        private int GetMovementSteps(string command)
        {
            var wordsToNumbers = new Dictionary<string, int>
            {
                {"ein", 1}, {"eins", 1}, {"einen", 1}, {"einem", 1},{"einschritt", 1}, {"zwei", 2}, {"drei", 3}, {"vier", 4}, 
            };

            var words = command.Split(' ');
            for (int i = 0; i < words.Length; i++)
            {
                if (words[i] == "schritt" || words[i] == "schritte")
                {
                    if (i > 0)
                    {
                        // Check if the previous word is a number or a number word
                        if (int.TryParse(words[i - 1], out int steps))
                        {
                            return steps;
                        }
                        else if (wordsToNumbers.TryGetValue(words[i - 1].ToLower(), out steps))
                        {
                            return steps;
                        }
                    }
                }
            }
            return 0; // Default to 0 steps if no number is found
        }



    }
}
