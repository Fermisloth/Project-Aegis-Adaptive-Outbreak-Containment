using UnityEngine;
 
 public class CreateLineMesh : MonoBehaviour
 {
     
     public GameObject go;
     void Start ()
     {
         
         go = GameObject.Find("Scores");
 

     }
     void Update ()
     {
         SceneVars sv = go.GetComponent<SceneVars>();
         
         var mesh = new Mesh();
         mesh.name = "My lines";
         mesh.vertices = new Vector3[] {
             new Vector3((sv.TotalInfected/10),0,0), new Vector3(2,0,0),
             new Vector3(0,0,0), new Vector3(0,2,0),
             new Vector3(0,2,0), new Vector3(2,2,0),
         };
         mesh.colors = new Color[] {
             Color.red, Color.red,
             Color.blue, Color.blue,
             Color.green, Color.yellow
         };
 
         mesh.SetIndices(new int[] {0,1, 2,3, 4,5 }, MeshTopology.Lines, 0, true);
 
         GetComponent<MeshFilter>().sharedMesh = mesh;
     }
 }