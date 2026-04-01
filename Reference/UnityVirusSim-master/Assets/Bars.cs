using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bars : MonoBehaviour
{
    public GameObject go;
    public GameObject bar;
    public GameObject NewObject;
    public float TempLength;
    public float barScale;
    // Start is called before the first frame update
    void Start()
    {
    NewObject = Instantiate(bar, new Vector3(5f,8.5f, 0), Quaternion.Euler(-90,0,0));

        GameObject go = GameObject.Find("Scores");
        SceneVars sv = go.GetComponent<SceneVars>();
        TempLength = 1;
    }

    // Update is called once per frame
    void Update()
    {
        SceneVars sv = go.GetComponent<SceneVars>();
        int barLength = sv.TotalBlobs - sv.TotalDead;
        //Debug.Log(barLength);
        barScale = (float)barLength / (float)sv.TotalBlobs;
        TempLength=TempLength - 0.01f;
        NewObject.transform.localScale = new Vector3(barScale, 1, 1);
        NewObject.transform.position = new Vector3(5f-(barScale/2),8.5f,0);
        
    }
}
