using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseHandler : MonoBehaviour
{
    [SerializeField] private GameObject pauseCanvasParent;

    private bool paused = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (InputHandler.Instance.Menu.down)
        {
            paused = !paused;

            if (paused)
                Time.timeScale = 0;
            else
                Time.timeScale = 1;

            pauseCanvasParent.SetActive(paused);
        }

    }

    public void Resume()
    {
        paused = false;
        Time.timeScale = 1;

        pauseCanvasParent.SetActive(paused);

    }
}
