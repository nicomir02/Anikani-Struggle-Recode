using System.Collections.Generic;
using UnityEngine;
using Pv.Unity;
namespace HelperScripts

{
    public class VoiceRecognition : MonoBehaviour
    {
        private RhinoManager _rhinoManager;
        private const string ACCESS_KEY = "VlY6Wf6Y287CQj0X/JNmzk39F3UyLZ0f+4Bth0EzH4icSZXuBVWdVw=="; // AccessKey obtained from Picovoice Console

        void Start()
        {
            // Initialize Rhino
            string contextPath = "Assets/StreamingAssets/contexts/mac/VoiceRecognitionMovement.rhn";
            _rhinoManager = RhinoManager.Create(ACCESS_KEY, contextPath, OnInferenceResult);
        }

        void OnDestroy()
        {
            // Cleanup
            _rhinoManager.Delete();
        }

        public void StartListening()
        {
            Debug.Log("Voice recognition started. Listening for commands...");
            // Start processing voice input
            _rhinoManager.Process();
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
            }
            else
            {
                Debug.Log("Command not understood.");
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