using UnityEngine;

public class HeatmapController : MonoBehaviour
{
    public SimulationManager manager;
    private Texture2D heatmapTex;
    private Color[] colors;
    
    private void Start()
    {
        heatmapTex = new Texture2D(20, 20);
        heatmapTex.filterMode = FilterMode.Bilinear;
        colors = new Color[20 * 20];
        
        var rend = GetComponent<Renderer>();
        if(rend)
        {
            rend.material.mainTexture = heatmapTex;
        }
        
        if (manager == null) manager = FindAnyObjectByType<SimulationManager>();
        if(manager != null) manager.OnTick += UpdateHeatmap;
    }
    
    private void UpdateHeatmap()
    {
        if(manager == null || manager.model == null) return;
        
        // Clear color array to green (low risk)
        for(int i=0; i<colors.Length; i++) colors[i] = Color.green;
        
        // Project agent positions to 20x20 grid
        // Simple scaling: assuming agent positions are between -10 and 10
        foreach(var agent in manager.model.agents)
        {
            if (agent.CurrentState == InfectionState.Infectious || agent.CurrentState == InfectionState.Exposed)
            {
                int gx = Mathf.FloorToInt((agent.transform.position.x + 10) / 20f * 20f);
                int gz = Mathf.FloorToInt((agent.transform.position.z + 10) / 20f * 20f);
                
                gx = Mathf.Clamp(gx, 0, 19);
                gz = Mathf.Clamp(gz, 0, 19);
                
                colors[gz * 20 + gx] = Color.red; // Add density
            }
        }
        
        heatmapTex.SetPixels(colors);
        heatmapTex.Apply();
    }
}
