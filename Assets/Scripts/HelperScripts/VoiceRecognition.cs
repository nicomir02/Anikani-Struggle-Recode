using System.Collections.Generic;
using UnityEngine;
using Pv.Unity;
namespace HelperScriptswe    

{
    public class VoiceRecognition : MonoBehaviour
    {
        private RhinoManager _rhinoManager;
        private const string ACCESS_KEY = "VlY6Wf6Y287CQj0X/JNmzk39F3UyLZ0f+4Bth0EzH4icSZXuBVWdVw=="; // AccessKey obtained from Picovoice Console

        void Start()
        {
            InitializeRhinoManager();
        }

        void OnDestroy()
        {
            _rhinoManager.Delete();
        }
        
        private void InitializeRhinoManager()
        {
            string contextPath = "Assets/StreamingAssets/contexts/mac/VoiceRecognitionMovement.rhn";
            _rhinoManager = RhinoManager.Create(ACCESS_KEY, contextPath, OnInferenceResult);
        }

        public void StartListening()
        {
            if (_rhinoManager != null && !_rhinoManager.IsRecording)
            {
                Debug.Log("Voice recognition started. Listening for commands...");
                _rhinoManager.Process();
            }
        }
        
        public void StopAndResetListening()
        {
            if (_rhinoManager != null)
            {
                _rhinoManager.Delete(); // Free the resources
                InitializeRhinoManager(); // Reinitialize to start listening again
                Debug.Log("Voice recognition stopped and reset.");
            }
        }

        private void OnInferenceResult(Inference inference)
        {
            if (inference.IsUnderstood)
            {
                string intent = inference.Intent;
                Debug.Log($"Command processed. Intent recognized: {intent}");

                Unit selectedUnit = FindSelectedUnit();
                if (selectedUnit != null)
                {
                    Vector3Int moveDirection = GetMoveDirection(intent);
                    Vector3Int targetPosition = selectedUnit.currentPosition + moveDirection;
                    selectedUnit.schritteGehen(targetPosition);
                }
                else
                {
                    Debug.Log("No unit selected.");
                }
                StopAndResetListening();
            }
            else
            {
                Debug.Log("Command not understood.");
                StopAndResetListening();
            }
        }

        private Unit FindSelectedUnit()
        {
            foreach (Unit unit in FindObjectsOfType<Unit>())
            {
                if (unit.m_selected) // Assuming m_selected indicates if the unit is selected
                {
                    return unit;
                }
            }
            return null;
        }

        private Vector3Int GetMoveDirection(string intent)
        {
            switch (intent.ToLower())
            {
                case "up":
                    return Vector3Int.up;
                case "down":
                    return Vector3Int.down;
                case "left":
                    return Vector3Int.left;
                case "right":
                    return Vector3Int.right;
                default:
                    return Vector3Int.zero;
            }
        }
    }
}