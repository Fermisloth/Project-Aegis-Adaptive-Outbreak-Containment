using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.IO;

public class SceneVars : MonoBehaviour
{
    [Header("Simulation parameters")]
    public int TotalBlobs = 150;
    public float xSize = 8;
    public float ySize = 8;
    public float xOrigin = 0;
    public float yOrigin = 0;
    public bool Continuous = false;

    [Header("Virus Parameters")]
    public int PCInfected = 20;
    public int ImmunityPeriod = 200;
    public float RecoveryPeriod = 7;
    public float InfectiousPeriod = 20;
    public int InfectionProb = 33;

    [Header("Spontaneous Reinfection")]

    public int SInfectionCh10000 = 1;
    public float DelayReinfect = 30;
    public int DelayReinfectTimes = 10;
    public int PCDelayedReinfect = 0;

    [Header("Self Isolation")]
    public bool LockdownOnLevel = false;
    public int LockDownLevel = 30;
    public int PCSelfIsolating = 50;
    public float SelfIsolationLength = 7;
    public int SelfIsolationTimes = 2;
    public float SelfIsolationBreak = 7;
    public int SelfIsolationDelay = 0;


    [Header("Masks")]
    public int PCMasks = 33;
    public int PCMaskInfect = 20;
    public int PCMaskTransmit = 20;

    [Header("Social Distancing")]
    public float PCSD = 0;

    [Header("Reporting Vars")]
    public float ReportPeriod = 4;

    [Header("Percentage and report calcs")]
    public float PCDead = 0;
    public float PCInfectedCurrent;
    public float PCHighInfected;
    public int HighInfections = 0;
    public float AverageR0 = 0;

    [Header("Runtime Variables")]
    public List<GameObject> Cells;
    public List<int> Transmissions;
    public float CurrentTick = 0;
    public int CurrentDay = 0;
    public int TotalPeople;
    public int TotalImmune = 0;
    public bool GameEnd;
    public int EndDay;
    public int TotalDead = 0;
    public int TotalAlive = 0;
    public int TotalInfected = 0;
    public int BlobHealth;
    public bool SelfIsolationNow = false;
    public float NextLockdownDay = 0;
    public float SelfIsolationEnd;
    public int TotalNeverInfected = 0;
    public float NextReinfectday;
    public float NextStatsCheckDay;

    public bool Reinfection;
    public bool BeginLockdown = false;
    public float BeginLockdownTime;
    public bool EndLockdown = false;
    public float EndLockdownTime;



    [Header("Object References")]
    public GameObject go;

    [Header("Deprecated")]
    public int LastDay = 0;
    public int NumberOf100Days;
    public int PCAtRisk = 20;
    public bool New100 = true;
    public bool New100Latch = false;
    public float New100Tick = 0;
    public int AtRiskDailyMort = 20;
    public float BlobScale = 0.3f;

    StreamWriter writer;

    // Start is called before the first frame update
    void Start()
    {

        // SelfIsolationNow = true;
        if (SelfIsolationTimes > 0 && SelfIsolationDelay < 0 && LockdownOnLevel == false)
        {
            SelfIsolationNow = true;
            SelfIsolationEnd = +SelfIsolationLength;
        }
        else
        {
            SelfIsolationNow = false;
        }
        NextLockdownDay = CurrentDay + SelfIsolationDelay;

        NextReinfectday = CurrentDay + DelayReinfect;

        GameEnd = false;
        string path = "./output.txt";
        writer = new StreamWriter(path, true);
        NextStatsCheckDay+=ReportPeriod;

    }

    // Update is called once per frame
    void Update()
    {
        //Update Timer
        if (CurrentDay > NextStatsCheckDay)
        {
            NextStatsCheckDay+=ReportPeriod;


            //calculate R0

            int CurrentNumberOfBlobs = Cells.Count;

            for (int i = 0; i < CurrentNumberOfBlobs; i++)
            {
                int NumberOfInfections = Cells[i].GetComponent<Movement>().TotalTimesSpread;
                Transmissions.Add(NumberOfInfections);
                //writer.WriteLine(NumberOfInfections);
            }
            

            AverageR0 = (float)Transmissions.Average();
            //writer.WriteLine("Average: " +AverageR0 );
            //writer.Close();
        }

        // calculate percentages
        PCDead = (float)((float)TotalDead / (float)TotalBlobs) * 100f; // calculate percentage dead.    
        PCInfectedCurrent = (float)((float)TotalInfected / (float)TotalBlobs) * 100f;
        PCHighInfected = (float)((float)HighInfections / (float)TotalBlobs) * 100f;


        //handle keyinput
        if (Input.GetKey("escape"))
        {
            Application.Quit();
        }

        if (GameEnd == false) { CurrentTick = CurrentTick + (1 * Time.deltaTime); } // current tick
        CurrentDay = (int)CurrentTick;
        if (CurrentDay > LastDay) //Update day counter
        {

            LastDay = CurrentDay;

        }

        //implement self isolation lockdown

        if (LockdownOnLevel == false)
        {
            if (SelfIsolationNow == false && SelfIsolationTimes > 0 && CurrentTick > NextLockdownDay)
            {
                SelfIsolationNow = true;
                SelfIsolationTimes--;
                if (SelfIsolationTimes > 0)
                {
                    SelfIsolationEnd = CurrentTick + SelfIsolationLength;
                }

            }
            if (SelfIsolationNow == true && CurrentTick > SelfIsolationEnd)
            {
                SelfIsolationNow = false;
                if (SelfIsolationTimes > 0)
                {
                    NextLockdownDay = CurrentTick + SelfIsolationBreak;
                }
            }
        }

        //implement automatic lockdown on level
        if (LockdownOnLevel == true)
        {
            if (PCInfectedCurrent > LockDownLevel && BeginLockdown == false)
            {
                BeginLockdown = true;
                BeginLockdownTime = CurrentTick + 1;
                EndLockdownTime = CurrentTick + 1;
            }
            if (PCInfectedCurrent > LockDownLevel)
            {
                EndLockdownTime = CurrentTick + 1;
            }
        }

        if (BeginLockdownTime > CurrentTick && EndLockdownTime < CurrentTick)
        {
            Debug.Log("Start Lockdown");
            SelfIsolationNow = true;
            BeginLockdownTime = 0;
            EndLockdownTime = CurrentTick + 1;
        }
        if (EndLockdownTime > CurrentTick)
        {
            SelfIsolationNow = false;
            Debug.Log("stop Lockdown");
            EndLockdownTime = 0;
            BeginLockdown = false;
            BeginLockdownTime = 0;
        }

        NumberOf100Days = (CurrentDay / 100); // calculate how many hundred days

        // delayed spontaneous infection
        if (TotalInfected > HighInfections)
        {
            HighInfections = TotalInfected;
        }

        if (TotalInfected == 0 && Continuous == false)
        {
            EndDay = CurrentDay;
            GameEnd = true;
        }


    }
    public void adjustInfectiousPeriod(float IP) // not used
    {
        InfectiousPeriod = IP;
    }
}
