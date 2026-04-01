using UnityEngine;

[CreateAssetMenu(fileName = "SimConfig", menuName = "Simulation/Config")]
public class SimulationConfig : ScriptableObject
{
    public float transmissionRate = 0.8f; // β (Increased for better spread)
    public float incubationRate = 0.4f;    // σ
    public float recoveryRate = 0.05f;      // γ (Decreased to prevent instant recovery)
    public float quarantineRate = 0.02f;   // δ quarantine rate organically
    public float mortalityRate = 0.05f;    // disease-induced mortality rate
    public float socialDistancing = 0.0f;  // u (0 = no intervention, 1 = total isolation)
    public float vaccinationRate = 0.0f;   // \nu (vaccination rate)
    public float interactionRadius = 2.0f;

    public int initialInfectedCount = 5;   // Chooseable number of starting infected agents
    public int initialBudget = 5000;
    public int hospitalCapacity = 50; 
    
    public int costQuarantine = 10;
    public int costVaccination = 50;
}
