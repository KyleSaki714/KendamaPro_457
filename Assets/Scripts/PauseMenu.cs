using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{

    public void OnPlayButton()
    {
        GameManager.Instance.PauseGame();
    }

    public void OnQuitButton()
    {
        SceneManager.LoadScene(0);
    }
}
