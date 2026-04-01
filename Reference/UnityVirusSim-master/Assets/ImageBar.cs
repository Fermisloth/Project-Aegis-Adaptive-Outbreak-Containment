using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageBar : MonoBehaviour
{
    public GameObject go;
    public Image OverallBar;
    public float OverallHealth;
    // Start is called before the first frame update
    void Start()
    {
        GameObject go = GameObject.Find("Scores");
        SceneVars sv = go.GetComponent<SceneVars>();
    }

    // Update is called once per frame
    void Update()
    {
        SceneVars sv = go.GetComponent<SceneVars>();

        OverallHealth = ((100f-sv.PCDead) / 100);
        OverallBar.fillAmount = OverallHealth;
        
    }
}
