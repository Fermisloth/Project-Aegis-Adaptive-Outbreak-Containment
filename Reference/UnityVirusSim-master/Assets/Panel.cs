using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Panel : MonoBehaviour
{
    public int ExampleScore=0;
    private Text exampleText;
    public GameObject go;
    public bool FileWritten=false;
    // Start is called before the first frame update
    void Start()
    {
        GameObject go = GameObject.Find("Scores");
        SceneVars sv = go.GetComponent<SceneVars>();
        exampleText = GetComponent<Text>();
        ExampleScore=0;
    }

    // Update is called once per frame
    void Update()
    {
        int x=1;
        SceneVars sv = go.GetComponent<SceneVars>();
        exampleText = GetComponent<Text>();
        string DayText="Current Day: " + sv.CurrentDay + "\n";
        string AliveText = "Total alive: " + sv.TotalAlive + "\n";

        double RoundedPCInfected = System.Math.Round (sv.PCInfectedCurrent,1);
        string PCInfected = "Percent infected: " + RoundedPCInfected + "\n";

        string NLockdown;
        if (sv.SelfIsolationTimes == 0) {NLockdown="Next lockdown: " + "NA" + "\n";}
        else {NLockdown="Next lockdown: " + (int)sv.NextLockdownDay + "\n";}
        
        double RoundedPCDead = System.Math.Round (sv.PCDead,1);
        string DeadText = "Percentage dead: " + RoundedPCDead  + "%" + "\n";
        
        double RoundedPCHighInfected = System.Math.Round (sv.PCHighInfected,1);
        string HighInfected = "High infected: " + RoundedPCHighInfected + "%" + "\n";

        double R0 = System.Math.Round (sv.AverageR0,2);
        string R0Number = "R0: " + R0 + "%" + "\n";

        string HighText="High infected: " + sv.HighInfections + "\n";
        exampleText.text = DayText+PCInfected+HighInfected+R0Number+AliveText+DeadText+HighText+NLockdown+AliveText;
    }
}
