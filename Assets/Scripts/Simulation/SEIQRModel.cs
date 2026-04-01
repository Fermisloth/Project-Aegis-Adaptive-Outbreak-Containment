using UnityEngine;
using System.Collections.Generic;

public class SEIQRModel : MonoBehaviour
{
    public SimulationConfig config;
    public List<HealthState> agents;

    public void Setup(List<HealthState> generatedAgents, SimulationConfig simConfig)
    {
        agents = generatedAgents;
        config = simConfig;
    }

    public void Tick()
    {
        if (config == null || agents == null) return;

        float dt = Time.fixedDeltaTime; // Using fixed time for predictable simulation step
        List<HealthState> iterAgents = new List<HealthState>(agents);

        foreach (var agent in iterAgents)
        {
            if (agent.CurrentState == InfectionState.Susceptible)
            {
                int contactCount = CountInfectiousContacts(agent);
                if (contactCount > 0)
                {
                    float transmissionProb = 1f - Mathf.Pow(1f - config.transmissionRate, dt * contactCount);
                    transmissionProb *= agent.SusceptibilityMultiplier; // Apply Demographic Susceptibility
                    if (UnityEngine.Random.value < transmissionProb)
                    {
                        agent.ChangeState(InfectionState.Exposed);
                    }
                }
            }
            else if (agent.CurrentState == InfectionState.Exposed)
            {
                if (UnityEngine.Random.value < config.incubationRate * dt)
                {
                    agent.ChangeState(InfectionState.Infectious);
                }
            }
            else if (agent.CurrentState == InfectionState.Infectious)
            {
                if (UnityEngine.Random.value < config.recoveryRate * dt)
                {
                    if (UnityEngine.Random.value < config.mortalityRate * agent.MortalityMultiplier)
                    {
                        agent.ChangeState(InfectionState.Dead);
                    }
                    else
                    {
                        agent.ChangeState(InfectionState.Removed);
                    }
                }
                // Check if they get quarantined organically
                else if(UnityEngine.Random.value < config.quarantineRate * dt)
                {
                    agent.ChangeState(InfectionState.Quarantined);
                }
            }
            else if (agent.CurrentState == InfectionState.Quarantined)
            {
                if (UnityEngine.Random.value < config.recoveryRate * dt)
                {
                    if (UnityEngine.Random.value < config.mortalityRate * agent.MortalityMultiplier)
                    {
                        agent.ChangeState(InfectionState.Dead);
                    }
                    else
                    {
                        agent.ChangeState(InfectionState.Removed);
                    }
                }
            }
        }
    }

    private int CountInfectiousContacts(HealthState agent)
    {
        int count = 0;
        foreach (var other in agents)
        {
            if (other == agent) continue;
            if (other.CurrentState == InfectionState.Infectious)
            {
                if (Vector3.Distance(agent.transform.position, other.transform.position) <= config.interactionRadius)
                {
                    count++;
                }
            }
        }
        return count;
    }
}
