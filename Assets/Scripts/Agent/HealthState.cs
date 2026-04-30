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
    
    // Set by spawner before Start() runs via direct field assignment
    [HideInInspector] public bool IsChild = false;

    public event Action<InfectionState> OnStateChanged;

    // Cached per-instance materials so we can tint without affecting shared assets
    private Material[] instanceMaterials;

    private void Start()
    {
        // Assign age based on whether spawner flagged as child
        if (IsChild)
            Age = Random.Range(3, 12);
        else
            Age = Random.Range(18, 85);

        AgentGender = (Random.value > 0.5f) ? Gender.Male : Gender.Female;

        // Susceptibility / Mortality multipliers
        if (Age < 12)
        {
            SusceptibilityMultiplier = 1.5f;
            MortalityMultiplier = 0.5f;
        }
        else if (Age > 65)
        {
            SusceptibilityMultiplier = 1.8f;
            MortalityMultiplier = 2.5f;
        }
        else
        {
            SusceptibilityMultiplier = 1.0f;
            MortalityMultiplier = 1.0f;
        }

        // Cache per-instance material copies for all renderers in this character
        CacheInstanceMaterials();
        
        // Apply initial colour based on CurrentState
        UpdateColor();
    }

    private void CacheInstanceMaterials()
    {
        var renderers = GetComponentsInChildren<Renderer>();
        var matList = new System.Collections.Generic.List<Material>();
        foreach (var r in renderers)
        {
            // Create per-instance copies so agents have independent colours
            var mats = r.materials; // .materials already returns copies
            r.materials = mats;
            matList.AddRange(mats);
        }
        instanceMaterials = matList.ToArray();
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
        if (instanceMaterials == null || instanceMaterials.Length == 0) return;

        Color stateColor = Color.white; // Default

        switch (CurrentState)
        {
            case InfectionState.Susceptible: stateColor = new Color(0f, 1f, 0.5f); break;
            case InfectionState.Exposed:     stateColor = new Color(1f, 0.8f, 0f); break;
            case InfectionState.Infectious:  stateColor = Color.red;               break;
            case InfectionState.Quarantined: stateColor = Color.blue;              break;
            case InfectionState.Removed:     stateColor = Color.gray;              break;
            case InfectionState.Dead:        stateColor = Color.black;             break;
        }

        // Create a 50% tint so the original texture isn't completely hidden
        Color tintColor = Color.Lerp(Color.white, stateColor, 0.5f);
        
        // Add a soft emission for visibility
        Color emissionColor = stateColor * 0.4f;

        foreach (var mat in instanceMaterials)
        {
            if (mat != null)
            {
                // URP uses _BaseMap for texture, _BaseColor for tint
                if (mat.HasProperty("_BaseColor"))
                {
                    mat.SetColor("_BaseColor", tintColor);
                }
                else if (mat.HasProperty("_Color"))
                {
                    mat.SetColor("_Color", tintColor);
                }

                // Enable emission so the state is clear even in shadows
                if (mat.HasProperty("_EmissionColor"))
                {
                    mat.EnableKeyword("_EMISSION");
                    mat.SetColor("_EmissionColor", emissionColor);
                }
            }
        }
    }
}
