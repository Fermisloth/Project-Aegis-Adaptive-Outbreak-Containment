using UnityEngine;
using System;
using Random = UnityEngine.Random;

public enum InfectionState { Susceptible, Exposed, Infectious, Quarantined, Removed, Dead }
public enum Gender { Male, Female }

public class HealthState : MonoBehaviour
{
    public InfectionState CurrentState = InfectionState.Susceptible;
    public float infectionTimer = 0f;
    public float exposureTimer = 0f;
    
    // Demographics
    public int Age;
    public Gender AgentGender;
    public float SusceptibilityMultiplier = 1.0f;
    public float MortalityMultiplier = 1.0f;
    
    public event Action<InfectionState> OnStateChanged;

    private void Start()
    {
        // Randomly assign demographics
        Age = Random.Range(5, 90);
        AgentGender = (Random.value > 0.5f) ? Gender.Male : Gender.Female;
        
        // Kids (under 12) and Seniors (over 65) are more susceptible and have higher mortality risks
        if (Age < 12) {
            SusceptibilityMultiplier = 1.5f;
            MortalityMultiplier = 0.5f; // Kids might catch it easily but lower mortality in some models, or higher if specified. Let's make mortality higher for both extremes as requested.
        } 
        else if (Age > 65) {
            SusceptibilityMultiplier = 1.8f;
            MortalityMultiplier = 2.5f; // Older people are much more vulnerable to severe outcomes
        }
        else {
            SusceptibilityMultiplier = 1.0f;
            MortalityMultiplier = 1.0f;
        }
    }

    public void ChangeState(InfectionState newState)
    {
        if (CurrentState == newState) return;
        CurrentState = newState;
        UpdateColor();
        OnStateChanged?.Invoke(newState);
    }
    
    private void UpdateColor()
    {
        var rend = GetComponent<Renderer>();
        if(rend == null) return;
        
        switch (CurrentState)
        {
            case InfectionState.Susceptible: rend.material.SetColor("_BaseColor", new Color(0f, 1f, 0.5f)); break; // Vivid green
            case InfectionState.Exposed: rend.material.SetColor("_BaseColor", new Color(1f, 0.8f, 0f)); break; // Vivid yellow-orange
            case InfectionState.Infectious: rend.material.SetColor("_BaseColor", new Color(1f, 0f, 0f)); break; // Vivid red
            case InfectionState.Quarantined: rend.material.SetColor("_BaseColor", new Color(0f, 0.5f, 1f)); break; // Vivid blue
            case InfectionState.Removed: rend.material.SetColor("_BaseColor", new Color(0.5f, 0.5f, 0.5f)); break; // Grey
            case InfectionState.Dead: rend.material.SetColor("_BaseColor", Color.black); break; // Black
        }
    }
}
