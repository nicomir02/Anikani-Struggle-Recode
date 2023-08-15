using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Button startGame;
    [SerializeField] private Button stopGame;

    void Start() {
        startGame.onClick.AddListener(onStartMultiplayer);
    }

    void onStartMultiplayer() {
        SceneManager.LoadScene(1);
    }
}
