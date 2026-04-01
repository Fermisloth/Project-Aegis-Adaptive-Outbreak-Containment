using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class circleScript : MonoBehaviour
{

    public GameObject Scores;
    // Start is called before the first frame update
    
    void Start()
    {
    //ShowInfect.GetComponent<ParticleSystem>().enableEmission = true;
    }

    // Update is called once per frame
    void Update()

    {
        Color FinalColour;
        Movement mv = this.GetComponentInParent<Movement>();

        FinalColour = new Color(0, 1f, 0);

        if (mv.Mask == true) FinalColour = new Color(1, 1, 1);

        if (mv.Infected == true && mv.Mask == true) FinalColour = new Color(1f, 0.5f, 0.5f);

        if (mv.Immune == true) FinalColour = mv.ColImmune;
        if (mv.IsDead == true) FinalColour = mv.DeadColour;
        if (mv.Infectious == true && mv.Infected == false) FinalColour = new Color(1f, 0, 1);

        if (mv.Infected == true)
        {
            
            FinalColour = mv.ColInfected;
        }
        
        GetComponent<Renderer>().material.color = FinalColour;
    }
}
