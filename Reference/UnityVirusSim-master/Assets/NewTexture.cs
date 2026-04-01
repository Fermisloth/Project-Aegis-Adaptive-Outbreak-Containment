using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewTexture : MonoBehaviour
{
    public GameObject scores;
    public Texture2D texture;
    public int OldDeadHigh = 0;
    public int graphScale;
    public int tempCounter = 0;
    public int delay = 0;
    public int old100Day = 0;
    public int next100Day = 0;
    public float XCounter = 0;
    public int EndCounter = 0;
    public int BarCounter = 0;
    public float multiplier = 1;
    public int graphHeight = 300;

    // Start is called before the first frame update
    void Start()
    {
        scores = GameObject.Find("Scores");

        texture = new Texture2D(1024, 150);
        texture.filterMode = FilterMode.Point;
        for (int i = 0; i < 1024; i++)  // paint white background
        {
            for (int j = 0; j < graphHeight*graphHeight; j++)
            {
                texture.SetPixel(i, j, Color.white);
            }
        }

        EndCounter = 1;


    }

    // Update is called once per frame
    void Update()
    {
        SceneVars sv = scores.GetComponent<SceneVars>();
        int CurrentX = 0;
        int i, j, k, l, m, n;
        int graphLength = 1024;
        float CounterIncrement;

        CounterIncrement = 1 * (Time.deltaTime * 10);
        CounterIncrement = CounterIncrement * multiplier;
        CurrentX = (int)XCounter;
        if (sv.GameEnd == true)
        {
            CounterIncrement = 0;
        }
        XCounter = XCounter + CounterIncrement;

        graphScale = sv.TotalBlobs / graphHeight;
        graphScale = 2;
        GetComponent<Renderer>().material.mainTexture = texture;

        if (CurrentX > 1024) // Scale graph when end reached
        {
            Color pixel;
            for (i = 1; i < graphLength; i += 2)
            {
                for (j = 1; j < graphHeight; j++)
                {
                    pixel = texture.GetPixel(i, j);
                    texture.SetPixel(i / 2, j, pixel);
                }
            }

            for (i = (graphLength / 2); i < 1024; i++)  // Clean 50% of background
            {
                for (j = 0; j < graphHeight; j++)
                {
                    texture.SetPixel(i, j, Color.white);
                }
            }
            next100Day++;
            next100Day = next100Day * 2;
            XCounter = 512;
            EndCounter++;
            multiplier = multiplier / 2f;
        }

        /*if (sv.CurrentDay % 10 == 0)        // draw grey ten day bar
        {
            for (i = 0; i < graphHeight; i++) //draw bar
            {
                texture.SetPixel(CurrentX, i, new Color(0.7f, 0.7f, 0.7f, 1));

            }
        }*/
        if (sv.SelfIsolationNow)        // draw grey ten day bar
        {
            for (i = 0; i < graphHeight; i++) //draw bar
            {
                texture.SetPixel(CurrentX, i, new Color(0.5f, 0.5f, 0.5f, 1));

            }
        }

        for (i = CurrentX - 1; i < 1024; i++) //dead high line
        {
            texture.SetPixel(i, OldDeadHigh / graphScale, Color.white);
        }

        for (i = CurrentX; i < 1024; i++)
        {
            texture.SetPixel(i, sv.HighInfections / graphScale, Color.red);
        }
        OldDeadHigh = sv.HighInfections;

       for (k = 0; k < graphHeight / 10; k++) // horizontal lines
        {
            for (l = 0; l < 1024; l++)
            {
                texture.SetPixel(l, k * 10, Color.black);
            }
        }

        for (i = 0; i < (int)(sv.TotalImmune); i++) // draw immune
        {
            texture.SetPixel(CurrentX, i / graphScale, Color.blue);
        }

        for (i = 0; i < (int)(sv.TotalDead); i++)   //draw dead
        {
            texture.SetPixel(CurrentX, i / graphScale, Color.black);
        }

        for (i = 0; i < (int)(sv.TotalInfected); i++) //draw infection
        {
            texture.SetPixel(CurrentX, i / graphScale, Color.red);
        }


        if (sv.Reinfection)
        {
            for (i = 0; i < graphHeight; i++) //draw bar
            {
                texture.SetPixel(CurrentX, i, new Color(1, 0, 0, 1));
                texture.SetPixel(CurrentX+1, i, new Color(1, 0, 0, 1));
                sv.Reinfection = false;
            }
        }


        texture.SetPixel(CurrentX, sv.TotalDead / graphScale, Color.black);
        texture.SetPixel(CurrentX, sv.TotalImmune / graphScale, Color.blue);



        texture.Apply();



    }

}
