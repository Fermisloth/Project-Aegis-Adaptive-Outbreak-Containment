using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    public float BarScale = 1;
    public GameObject Scores;
    public Color HealthyColour;
    public Color SickColour;
    // Start is called before the first frame update
    void Start()
    {
        Scores=GameObject.Find("Scores");
    }

    // Update is called once per frame
    void Update()
    {
        //SceneVars mv = Scores.GetComponent<>();
        float health;
        // health = this.transform.parent.GetComponent<Movement>().BlobHealth;
        Movement mv =  this.GetComponentInParent<Movement>();
        //health = this.parent.GetComponent<Movement>().BlobHealth;
        health=mv.BlobHealth/100f;
        Vector3 Newscale = transform.localScale;
        transform.localScale = new Vector3 (health * BarScale, 0.15f, 1);

        GetComponent<Renderer>().material.color = Color.Lerp(SickColour,HealthyColour,health);
        //GetComponent<Renderer>().material.color = new Color(1f-health, 1f, 0);
    }
}
