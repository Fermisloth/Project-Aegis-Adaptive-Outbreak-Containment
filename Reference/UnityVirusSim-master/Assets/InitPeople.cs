using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitPeople : MonoBehaviour
{
    public GameObject myPrefab;
    public GameObject myPrefabSD;
    


    // Start is called before the first frame update
    void Start()
    {

        Random rnd = new Random();
        GameObject go = GameObject.Find("Scores");
        SceneVars sv = go.GetComponent<SceneVars>();
        int TotalBlobs = sv.TotalBlobs;
        GameObject PrefabChoice;

        for (int i = 0; i < TotalBlobs; i++)
        {
            sv.TotalAlive++;
            
            if (Random.Range (0, 100) < sv.PCSD)
            {PrefabChoice = myPrefabSD;}
            else
            {PrefabChoice = myPrefab;}
           
            //GameObject NewObject = Instantiate(PrefabChoice, new Vector3(Random.Range(sv.xOrigin, sv.xSize), Random.Range(sv.yOrigin, sv.ySize), 0), Quaternion.identity);
            sv.Cells.Add (Instantiate(PrefabChoice, new Vector3(Random.Range(sv.xOrigin, sv.xSize), Random.Range(sv.yOrigin, sv.ySize), 0), Quaternion.identity));
            sv.Cells[i].GetComponent<Movement>().xDirection = Random.Range(-1f, 1f);
            sv.Cells[i].GetComponent<Movement>().yDirection = Random.Range(-1f, 1f);
            sv.Cells[i].GetComponent<Movement>().BlobHealth = Random.Range(10, 101);
            //NewObject.transform.localScale = new Vector3(sv.BlobScale, sv.BlobScale, sv.BlobScale);
            sv.TotalNeverInfected++;

            if (Random.Range(0, 100) < sv.PCInfected)
            {
                //Debug.Log("Set infect on init");
                sv.Cells[i].GetComponent<Movement>().Infected = true;
                sv.Cells[i].GetComponent<Movement>().TickRecovered = sv.CurrentTick + Random.Range(1, (sv.RecoveryPeriod + 1)); //pre existing infection

                sv.TotalInfected++;
                sv.TotalNeverInfected--;
            }

            if (Random.Range(0, 100) < sv.PCMasks)
            {
                sv.Cells[i].GetComponent<Movement>().Mask = true;
            }
            else
            {
                sv.Cells[i].GetComponent<Movement>().Mask = false;
            }

            if (Random.Range(0, 100) < sv.PCSelfIsolating)
            {
                sv.Cells[i].GetComponent<Movement>().SelfIsolating = true;
            }
            else
            {
                sv.Cells[i].GetComponent<Movement>().SelfIsolating = false;
            }
        }
        

    }

    // Update is called once per frame
    void Update()
    {

    }
}
