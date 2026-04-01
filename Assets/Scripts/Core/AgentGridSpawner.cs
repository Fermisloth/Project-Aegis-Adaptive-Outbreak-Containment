using UnityEngine;
using System.Collections.Generic;

public class AgentGridSpawner : MonoBehaviour
{
    public GameObject agentPrefab;
    public int gridSize = 20;
    public float spacing = 3.5f;
    
    private List<HealthState> generatedAgents = new List<HealthState>();

    public SimulationManager manager;

    void Start()
    {
        int totalAgents = gridSize * gridSize;
        float cityScatter = 35f;
        
        for (int i = 0; i < totalAgents; i++)
        {
            float rx = Random.Range(-cityScatter, cityScatter);
            float rz = Random.Range(-cityScatter, cityScatter);
            Vector3 pos = new Vector3(rx, 0.5f, rz);
            
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.GetComponent<Renderer>().material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            go.transform.position = pos;
            go.transform.parent = this.transform;
            go.name = $"Agent_{i}";
            
            HealthState hs = go.AddComponent<HealthState>();
            go.AddComponent<Movement>();
            
            generatedAgents.Add(hs);
        }
        
        if (manager == null) manager = FindAnyObjectByType<SimulationManager>();

        // Setup initial infection (Random first patients)
        if(generatedAgents.Count > 0 && manager != null && manager.config != null)
        {
            int toInfect = Mathf.Min(manager.config.initialInfectedCount, generatedAgents.Count);
            for (int k = 0; k < toInfect; k++)
            {
                int attempts = 0;
                while (attempts < 50)
                {
                    int index = Random.Range(0, generatedAgents.Count);
                    if (generatedAgents[index].CurrentState == InfectionState.Susceptible)
                    {
                        generatedAgents[index].ChangeState(InfectionState.Infectious);
                        break;
                    }
                    attempts++;
                }
            }
        }
        
        if (manager != null) manager.Initialize(generatedAgents);
    }
}
