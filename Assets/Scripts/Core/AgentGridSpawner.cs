using UnityEngine;
using System.Collections.Generic;

public class AgentGridSpawner : MonoBehaviour
{
    [Header("Low Poly Character Prefabs")]
    [Tooltip("Drag in the 'normal man a/b/c' prefabs from DavidJalbert/LowPolyPeople/Prefabs")]
    public GameObject[] malePrefabs;
    [Tooltip("Drag in the 'normal woman a/b/c' prefabs from DavidJalbert/LowPolyPeople/Prefabs")]
    public GameObject[] femalePrefabs;

    [Header("Spawn Settings")]
    public int populationSize = 400;
    public float cityScatter = 35f;

    [Header("Demographic Proportions (0-1)")]
    [Range(0f, 0.3f)] public float childFraction = 0.15f;
    [Range(0f, 1f)] public float maleFraction = 0.5f;
    public float childScale = 0.55f;

    private List<HealthState> generatedAgents = new List<HealthState>();

    public SimulationManager manager;

    void Start()
    {
        SpawnAgents();
    }

    public void Respawn(int newPopulationSize, float newMaleFraction, float newChildFraction)
    {
        this.populationSize = newPopulationSize;
        this.maleFraction = newMaleFraction;
        this.childFraction = newChildFraction;
        
        // Destroy only the generated agents, leaving environment intact
        foreach (var agent in generatedAgents)
        {
            if (agent != null)
            {
                Destroy(agent.gameObject);
            }
        }
        generatedAgents.Clear();

        if (manager == null) manager = FindAnyObjectByType<SimulationManager>();
        
        // Ensure lockdown/quarantine is reset
        if (manager != null)
        {
            manager.lockdownTimer = 0;
            manager.quarantineTimer = 0;
        }

        SpawnAgents();
    }

    private void SpawnAgents()
    {
        int totalAgents = populationSize;

        bool hasMale = malePrefabs != null && malePrefabs.Length > 0;
        bool hasFemale = femalePrefabs != null && femalePrefabs.Length > 0;

        for (int i = 0; i < totalAgents; i++)
        {
            float rx = Random.Range(-cityScatter, cityScatter);
            float rz = Random.Range(-cityScatter, cityScatter);
            Vector3 pos = new Vector3(rx, 0f, rz);

            bool isChild = Random.value < childFraction;
            bool isMale  = Random.value < maleFraction;

            GameObject go;

            if (hasMale || hasFemale)
            {
                GameObject[] pool = null;
                if (isMale && hasMale) pool = malePrefabs;
                else if (!isMale && hasFemale) pool = femalePrefabs;
                else pool = hasMale ? malePrefabs : femalePrefabs;

                GameObject prefab = pool[Random.Range(0, pool.Length)];
                go = Instantiate(prefab, pos, Quaternion.Euler(0, Random.Range(0f, 360f), 0), this.transform);
            }
            else
            {
                go = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                go.transform.position = pos;
                go.transform.parent = this.transform;
                var mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
                go.GetComponent<Renderer>().sharedMaterial = mat;
            }

            go.name = $"Agent_{i}_{(isMale ? "M" : "F")}{(isChild ? "_kid" : "")}";

            if (isChild)
                go.transform.localScale = Vector3.one * childScale;

            HealthState hs = go.AddComponent<HealthState>();
            hs.IsChild = isChild;
            go.AddComponent<Movement>();

            generatedAgents.Add(hs);
        }

        if (manager == null) manager = FindAnyObjectByType<SimulationManager>();

        if (generatedAgents.Count > 0 && manager != null && manager.config != null)
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
