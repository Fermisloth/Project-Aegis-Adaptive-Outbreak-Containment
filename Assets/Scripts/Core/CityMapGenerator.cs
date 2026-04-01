using UnityEngine;

public class CityMapGenerator : MonoBehaviour
{
    public int numberOfBuildings = 16;
    public float citySize = 38f;

    public static System.Collections.Generic.List<Vector3> BuildingPositions = new System.Collections.Generic.List<Vector3>();

    void Start()
    {
        Material buildingMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        buildingMat.SetColor("_BaseColor", new Color(0.2f, 0.2f, 0.2f));
        
        Material barrierMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        barrierMat.SetColor("_BaseColor", new Color(0.05f, 0.3f, 0.4f, 0.6f));

        Material floorMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        floorMat.SetColor("_BaseColor", new Color(0.1f, 0.1f, 0.1f));

        // Delete old green plane if it exists
        GameObject oldGround = GameObject.Find("GroundPlane");
        if (oldGround != null) Destroy(oldGround);

        // Create large floor
        GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
        floor.name = "CityFloor";
        floor.transform.position = new Vector3(0, -0.5f, 0);
        floor.transform.localScale = new Vector3(citySize * 2.5f, 1f, citySize * 2.5f);
        floor.GetComponent<Renderer>().material = floorMat;
        floor.transform.parent = this.transform;

        // Generate Barriers (N, S, E, W borders to prevent escaping)
        float boundDist = citySize * 1.25f;
        float hHeight = 3f;
        CreateBarrier(new Vector3(0, hHeight, boundDist), new Vector3(boundDist * 2, hHeight * 2, 2f), barrierMat);
        CreateBarrier(new Vector3(0, hHeight, -boundDist), new Vector3(boundDist * 2, hHeight * 2, 2f), barrierMat);
        CreateBarrier(new Vector3(boundDist, hHeight, 0), new Vector3(2f, hHeight * 2, boundDist * 2), barrierMat);
        CreateBarrier(new Vector3(-boundDist, hHeight, 0), new Vector3(2f, hHeight * 2, boundDist * 2), barrierMat);

        for (int i = 0; i < numberOfBuildings; i++)
        {
            GameObject building = GameObject.CreatePrimitive(PrimitiveType.Cube);
            building.name = "Building_" + i;
            building.transform.parent = this.transform;

            // Organize buildings only around the edges so the center is clear for agents
            float rx, rz;
            if (Random.value > 0.5f) {
                rx = Random.Range(-citySize, citySize);
                rz = (Random.value > 0.5f) ? Random.Range(citySize - 5f, citySize) : Random.Range(-citySize, -citySize + 5f);
            } else {
                rz = Random.Range(-citySize, citySize);
                rx = (Random.value > 0.5f) ? Random.Range(citySize - 5f, citySize) : Random.Range(-citySize, -citySize + 5f);
            }
            
            float sx = Random.Range(3f, 7f);
            float sz = Random.Range(3f, 7f);
            float sy = Random.Range(4f, 15f);

            building.transform.position = new Vector3(rx, sy / 2f, rz);
            building.transform.localScale = new Vector3(sx, sy, sz);

            building.GetComponent<Renderer>().material = buildingMat;
            
            BuildingPositions.Add(new Vector3(rx, 0.5f, rz));
        }
    }

    private void CreateBarrier(Vector3 pos, Vector3 scale, Material mat)
    {
        GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wall.name = "BorderWall";
        wall.transform.position = pos;
        wall.transform.localScale = scale;
        wall.GetComponent<Renderer>().material = mat;
        wall.transform.parent = this.transform;
    }
}
