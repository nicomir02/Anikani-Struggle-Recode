using System.Collections.Generic;
using UnityEngine;
using Pv.Unity;
namespace HelperScripts

{
    public class VoiceRecognition : MonoBehaviour
    {
        private RhinoManager _rhinoManager;
        private const string ACCESS_KEY = "VlY6Wf6Y287CQj0X/JNmzk39F3UyLZ0f+4Bth0EzH4icSZXuBVWdVw=="; // AccessKey obtained from Picovoice Console (https://console.picovoice.ai/)


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

        private void OnInferenceResult(Inference inference)
        {
            if (inference.IsUnderstood)
            {
                // Handle the understood intent and slots
                string intent = inference.Intent;
                Debug.Log($"Intent: {intent}");

                foreach (KeyValuePair<string, string> slot in inference.Slots)
                {
                    Debug.Log($"{slot.Key}: {slot.Value}");
                }

                // Implement your actions based on the intent
                // For example: if (intent == "go left") { ... }
            }
            else
            {
                Debug.Log("Did not understand the command.");
            }
        }

        // Implement a method to start voice recognition
        public void StartListening()
        {
            Debug.Log("Button pressed, starting voice recognition.");
            Debug.Log("Picovoice processing started.");
            _rhinoManager.Process();
        }
    }
}