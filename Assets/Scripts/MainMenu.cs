using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// Main menu UI.
public class MainMenu : MonoBehaviour
{
    public Button playButton;
    public string scene_change = "SampleScene";

    // Start is called before the first frame update
    void Start()
    {
        playButton.onClick.AddListener(OpenGame);
    }

    void OpenGame()
    {
        SceneManager.LoadScene(scene_change);
    }
}
