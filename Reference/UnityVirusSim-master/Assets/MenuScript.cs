using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour
{
    public void StartSimulation()
    {
        SceneManager.LoadScene(1);
    }

    public void QuitApp()
    {
        Debug.Log("Quit!");
    }
}
