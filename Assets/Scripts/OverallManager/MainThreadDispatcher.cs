namespace OverallManager
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class MainThreadDispatcher : MonoBehaviour
    {
        private static readonly Queue<Action> executionQueue = new Queue<Action>();

        public static MainThreadDispatcher Instance { get; private set; }

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(this.gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void Update()
        {
            lock (executionQueue)
            {
                while (executionQueue.Count > 0)
                {
                    executionQueue.Dequeue().Invoke();
                }
            }
        }

        public void Enqueue(Action action)
        {
            lock (executionQueue)
            {
                executionQueue.Enqueue(action);
            }
        }

        public static void RunOnMainThread(Action action)
        {
            if (Instance != null)
            {
                Instance.Enqueue(action);
            }
        }
    }

}