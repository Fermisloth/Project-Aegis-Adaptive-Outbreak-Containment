using UnityEngine;
using System;
using System.Collections.Generic;

public class SimulationManager : MonoBehaviour
{
    public static SimulationManager Instance;
    
    public SimulationConfig config;
    public SEIQRModel model;
    
    public int stepCount = 0;
    public float remainingBudget;
    public int currentHospitalOccupancy = 0;
    public int maxHospitalOccupancy = 0;
    public int peakInfectious = 0;
    
    [Header("Telemetry")]
    public int countS, countE, countI, countQ, countR, countD;

    public Action OnTick;
    public Action<int> OnBudgetChanged;
    
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        
        if(config != null) remainingBudget = config.initialBudget;
    }
    
    public void Initialize(List<HealthState> agents)
    {
        if (model == null) model = gameObject.AddComponent<SEIQRModel>();
        model.Setup(agents, config);
        
        foreach (var agent in agents)
            agent.OnStateChanged += HandleStateChanged;
            
        // Setup initial Telemetry
        countS = countE = countI = countQ = countR = 0;
        foreach (var agent in agents)
            UpdateCountsForState(agent.CurrentState, 1);
    }
    
    private void FixedUpdate()
    {
        if (model == null || config == null) return;
        
        model.Tick();
        stepCount++;
        OnTick?.Invoke();
    }
    
    private void HandleStateChanged(InfectionState newState)
    {
        // Simple recalculate for safety (could be optimized)
        countS = countE = countI = countQ = countR = countD = 0;
        currentHospitalOccupancy = 0;
        
        foreach (var agent in model.agents)
        {
            UpdateCountsForState(agent.CurrentState, 1);
            if(agent.CurrentState == InfectionState.Quarantined || agent.CurrentState == InfectionState.Infectious)
            {
               // Simplification for hospital: if infectious/quarantined it might take up hospital beds
               // Based on project guidelines it's likely Quarantined or a subset of infectious
               if(agent.CurrentState == InfectionState.Quarantined) 
                   currentHospitalOccupancy++;
            }
        }
        
        if(currentHospitalOccupancy > maxHospitalOccupancy) maxHospitalOccupancy = currentHospitalOccupancy;
        if(countI > peakInfectious) peakInfectious = countI;
    }
    
    private void UpdateCountsForState(InfectionState state, int amount)
    {
        switch (state)
        {
            case InfectionState.Susceptible: countS += amount; break;
            case InfectionState.Exposed: countE += amount; break;
            case InfectionState.Infectious: countI += amount; break;
            case InfectionState.Quarantined: countQ += amount; break;
            case InfectionState.Removed: countR += amount; break;
            case InfectionState.Dead: countD += amount; break;
        }
    }
    
    public void ExecuteAction(int actionId)
    {
        // 0=No Action, 1=Regional Lockdown, 2=Targeted Vaccination
        if(actionId == 1) EnforceLockdown();
        if(actionId == 2) ApplyMassVaccination();
    }
    
    public void EnforceLockdown()
    {
        float cost = 500; // Flat cost for lockdown applied
        if (remainingBudget >= cost)
        {
            remainingBudget -= cost;
            OnBudgetChanged?.Invoke((int)remainingBudget);
            
            foreach (var agent in model.agents)
            {
                var mov = agent.GetComponent<Movement>();
                if(mov) mov.SetLockdown(true);
            }
            Debug.Log("Lockdown Enforced!");
        }
    }
    
    public void ApplyMassVaccination()
    {
        // Vaccinate up to 10 random susceptible per call
        int count = 10; 
        foreach (var agent in model.agents)
        {
            if (count <= 0) break;
            if (agent.CurrentState == InfectionState.Susceptible)
            {
                if (remainingBudget >= config.costVaccination)
                {
                    remainingBudget -= config.costVaccination;
                    agent.ChangeState(InfectionState.Removed); // Removed = Immune/Vaccinated
                    count--;
                }
            }
        }
        OnBudgetChanged?.Invoke((int)remainingBudget);
        Debug.Log("Vaccination applied to random agents.");
    }
}
