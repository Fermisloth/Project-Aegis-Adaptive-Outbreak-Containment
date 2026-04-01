using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SDBehavior : MonoBehaviour
{
    public GameObject Scores;
    public bool Blocked;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        Movement mv =  this.GetComponentInParent<Movement>();
        mv.IsSDBlocked = true;
    }
}
