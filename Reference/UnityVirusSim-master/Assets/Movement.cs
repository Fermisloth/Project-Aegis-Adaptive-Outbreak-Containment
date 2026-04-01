using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    // Start is called before the first frame update


    public Rigidbody rb;
    [Header("Main properties")]
    public bool Infected = false;
    public bool Immune = false;
    public bool IsDead = false;
    public float BlobHealth;
    public int TotalTimesSpread;
    public int RunningTotalTimesSpread;
    public bool Infectious;
    public bool Immobile;

    [Header("Init properties")]
    public bool SelfIsolating;
    public bool SD;
    public bool Mask;
    public float TickUninfectious;

    [Header("Running vars")]
    public float xDirection;
    public float yDirection;
    public int TicksInfected;
    public float TickRecovered;
    public bool ChangeDirection;
    public int ImmuneCounter;
    public float ImmuneExpires;
    public float NextHealthCheck = 0;
    public int TotalTimesInfected = 0;
    public float NextRefinfectDay;
    public float NextSReinfectDayCheck;

    public bool IsSDBlocked;
    public float IsSDBlockedTimer;

    [Header("Deprecated")]
    public int BlobSeed;
    public bool AtRisk;



    public ParticleSystem InfectPart;
    public ParticleSystem ShowInfect;

    public Color ColInfected, ColNormal, ColImmune, DeadColour;

    public GameObject go;

    void OnTriggerEnter2D(Collider2D other) // change to OnCollisionEnter
    {
        SceneVars sv = go.GetComponent<SceneVars>();
        ChangeDirection = true;
        if (IsDead == false && other.GetComponent<Movement>().Infected == true && Infected == false && Immune == false) // set infected
        {
            int OurInfectLevel = sv.InfectionProb;
            int OtherInfectLevel = sv.InfectionProb;
            if (other.GetComponent<Movement>().Mask == true)
            {
                OurInfectLevel = -sv.PCMaskTransmit;
            }

            if (Mask == true)
            {
                OurInfectLevel = -sv.PCMaskInfect;
            }

            Random rnd = new Random();
            if (Random.Range(0, 100) < OurInfectLevel)
            {
                BecomeInfected();
                other.GetComponent<Movement>().TotalTimesSpread++;
                other.GetComponent<Movement>().RunningTotalTimesSpread++;
                //Debug.Log("Transmission!");
            }

        }
        if (other.GetComponent<Movement>().IsDead == true)
        {
            ChangeDirection = false;
        }

    }

    void Start()
    {
        if (Infected) Infected = true;
        Random.seed = BlobSeed;
        rb = GetComponent<Rigidbody>();
        go = GameObject.Find("Scores");
        SceneVars sv = go.GetComponent<SceneVars>();
        NextRefinfectDay = sv.CurrentTick + sv.DelayReinfect;
        NextSReinfectDayCheck = sv.CurrentTick + 1;
        ShowInfect = Instantiate(InfectPart, transform.position, Quaternion.identity);
        ShowInfect.GetComponent<ParticleSystem>().Play();
        ShowInfect.transform.parent = gameObject.transform;
    }

    // Update is called once per frame
    void Update()
    {
        SceneVars sv = go.GetComponent<SceneVars>();
        float BoxXSize = sv.xSize;
        float BoxYSize = sv.ySize;
        float BoxXStart = sv.xOrigin;
        float BoxYStart = sv.yOrigin;
        int ChangeChance;


        //set colours
        float HealthDiv = BlobHealth / 100;
       
        //particle code
        if (Infected == true)
        {
            ShowInfect.GetComponent<ParticleSystem>().enableEmission = true;
        }
        else
        {
            ShowInfect.GetComponent<ParticleSystem>().enableEmission = false;
        }
        //ShowInfect.transform.position=transform.position;


        float BallSpeed = 0.7f;
        Random rnd = new Random();
        Vector3 PersonPosition = transform.position;


        if (PersonPosition.x > BoxXSize) { xDirection = xDirection - 0.7f; }
        if (PersonPosition.x < BoxXStart) { xDirection = xDirection + 0.7f; }
        if (PersonPosition.y > BoxYSize) { yDirection = yDirection - 0.7f; }
        if (PersonPosition.y < BoxYStart) { yDirection = yDirection + 0.7f; }

        float ChangeAmount = 0.3f;


        ChangeChance = Random.Range(1, 6);
        if (IsSDBlocked)
        {
            //ChangeChance = 1;
            //ChangeAmount = 0.1f;
            IsSDBlockedTimer = sv.CurrentTick + 1;
        }


        if (Random.Range(1, 5) == 1 || IsSDBlocked)
        {
            if (ChangeChance == 1) { xDirection = xDirection + ChangeAmount; }
            if (ChangeChance == 2) { xDirection = xDirection - ChangeAmount; }
        }

        if (Random.Range(1, 5) == 1 || IsSDBlocked)
        {
            if (ChangeChance == 1) { yDirection = yDirection + ChangeAmount; }
            if (ChangeChance == 2) { yDirection = yDirection - ChangeAmount; }
        }



        ChangeDirection = false;

        if (xDirection > 1) { xDirection = 1; }
        if (yDirection > 1) { yDirection = 1; }
        if (xDirection < -1) { xDirection = -1; }
        if (yDirection < -1) { yDirection = -1; }

        PersonPosition.x += (BallSpeed * xDirection * Time.deltaTime);
        PersonPosition.y += (BallSpeed * yDirection * Time.deltaTime);

        //immobilize blob
        Immobile = false;
        if (SelfIsolating == true && sv.SelfIsolationNow == true) { Immobile = true; }
        
        if (IsDead == true) { Immobile = true; }

        //perform movement
        
        if (Immobile == false) { transform.position = PersonPosition; }

        IsSDBlocked = false;

        //Monitor infection
        if (Infected == true && sv.CurrentTick > TickRecovered) //recovered - set immune
        {
            Infected = false;
            Immune = true;
            sv.TotalInfected--;
            sv.TotalImmune++;
            ImmuneExpires = sv.CurrentTick + sv.ImmunityPeriod;
            // BlobHealth = ((BlobHealth / 100f) * 66f);
        }

        //Apply period reinfection chance
        if (sv.CurrentTick > NextRefinfectDay)
        {
            if (Random.Range(0, 100) < sv.PCDelayedReinfect && Infected == false)
            {
                sv.Reinfection = true;
                BecomeInfected();
            }
            NextRefinfectDay = sv.CurrentTick + sv.DelayReinfect;
        }

        //Apply constant spontaneous reinfection chance
        if (sv.CurrentTick > NextSReinfectDayCheck)
        {
            if (Random.Range(0, 10000) < sv.SInfectionCh10000 && Immune == false && Infected == false)
            {
                BecomeInfected();
                sv.Reinfection = true;
            }
            NextSReinfectDayCheck++;
        }

        //Apply damage 
        if (Infected == true && sv.CurrentTick > NextHealthCheck && IsDead == false)
        {
            BlobHealth = BlobHealth - 15f / (BlobHealth / 2);
            NextHealthCheck++;
        }

        //Kill blob
        if (BlobHealth < 1 && IsDead == false)
        {
            SetDead();
        }

        //monitor immunity
        if (sv.CurrentTick > ImmuneExpires && Immune == true)
        {
            Immune = false;
            sv.TotalImmune--;
        }

        //monitor infectiousness
        if (sv.CurrentTick > TickUninfectious)
        {
            Infectious = false;
        }
    }
    void SetDead()
    {
        SceneVars sv = go.GetComponent<SceneVars>();
        IsDead = true;
        Infected = false;
        Infectious = false;

        sv.TotalAlive--;
        sv.TotalDead++;
        sv.TotalInfected--;
        sv.TotalNeverInfected--;
        
        BlobHealth = 0;
    }
    void BecomeInfected()
    {
        SceneVars sv = go.GetComponent<SceneVars>();

        Infected = true;
        Infectious = true;
        sv.TotalInfected++;
        TotalTimesInfected++;
        TickRecovered = sv.CurrentTick + sv.RecoveryPeriod;
        NextHealthCheck = sv.CurrentTick + 1;
        TickUninfectious = sv.CurrentTick + sv.InfectiousPeriod;
    }
}