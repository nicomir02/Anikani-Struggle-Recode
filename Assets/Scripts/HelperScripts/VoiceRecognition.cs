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
            
            SimpleSentimentAnalysis sentimentAnalysisComponent = FindObjectOfType<SimpleSentimentAnalysis>();
            if (sentimentAnalysisComponent != null)
            {
                sentimentAnalysisComponent.AnalyzeSentiment(command);
            }
            else
            {
                Debug.LogError("SimpleSentimentAnalysis component not found.");
            }

            string[] commands = command.Split(new string[] { "and" }, StringSplitOptions.RemoveEmptyEntries);
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
            
                    await Task.Delay(500); 
                }
            }
            else
            {
                Debug.Log("Invalid move command.");
            }
        }

        private Vector3Int GetMoveDirection(string command)
        {
            if (command.Contains("up")) return Vector3Int.up;
            if (command.Contains("down")) return Vector3Int.down;
            if (command.Contains("left")) return Vector3Int.left;
            if (command.Contains("right")) return Vector3Int.right;
            return Vector3Int.zero;
        }

        private int GetMovementSteps(string command)
        {
            var oneStepWords = new HashSet<string> 
            { 
                "one", "on", "won", "wun", "uhn", "un", "an", "han", "hone", "oan" 
            };

            var words = command.Split(' ');
            foreach (var word in words)
            {
                if (oneStepWords.Contains(word.ToLower()))
                {
                    return 1; 
                }
            }

            var wordsToNumbers = new Dictionary<string, int>
            {
                {"two", 2}, {"to", 2}, {"too", 2}, {"tou", 2}, {"tu", 2}, 
                {"tew", 2}, {"tue", 2}, {"do", 2}, {"doo", 2}, {"dto", 2},
                {"three", 3}, {"tree", 3}, {"thri", 3}, {"free", 3}, {"phree", 3}, 
                {"thee", 3}, {"thre", 3}, {"trhee", 3}, {"threa", 3}, {"tharee", 3},
                {"four", 4}, {"for", 4}, {"fore", 4}, {"phor", 4}, {"faur", 4}, 
                {"foor", 4}, {"fawr", 4}, {"fower", 4}, {"fouhr", 4}, {"foar", 4}
            };

            for (int i = 0; i < words.Length; i++)
            {
                if (words[i] == "step" || words[i] == "steps")
                {
                    if (i > 0 && wordsToNumbers.TryGetValue(words[i - 1].ToLower(), out int steps))
                    {
                        return steps;
                    }
                }
            }

            return 0; 
        }

        public void HandlePositiveSentiment()
        {
            Debug.Log("Positive");
        }

        public void HandleNegativeSentiment()
        {
            Debug.Log("Negative");

        }

        public void HandleNeutralSentiment()
        {
            Debug.Log("Neutral");
        }


    }
}
