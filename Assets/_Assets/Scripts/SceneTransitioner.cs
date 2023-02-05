using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class SceneTransitioner : MonoBehaviour
{
    public static void ToMainMenu()
    {
        SceneManager.LoadScene(0);
    }

    public static void StartGame()
    {
        SceneManager.LoadScene(1);
    }

    public static void QuitGame()
    {
        Application.Quit();
    }
}
