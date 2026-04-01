using UnityEngine;
using System.Collections.Generic;

public class EpidemicChart3D : MonoBehaviour
{
    public SimulationManager manager;
    
    public LineRenderer lineS;
    public LineRenderer lineI;
    public LineRenderer lineR;
    
    public int maxHistory = 100;
    public float width = 10f;
    public float height = 5f;
    
    // Using queues to maintain history
    private Queue<int> histS = new Queue<int>();
    private Queue<int> histI = new Queue<int>();
    private Queue<int> histR = new Queue<int>();

    void Start()
    {
        if (manager == null) manager = FindAnyObjectByType<SimulationManager>();
        
        var lrs = GetComponentsInChildren<LineRenderer>();
        if(lrs.Length >= 3)
        {
            lineS = lrs[0];
            lineI = lrs[1];
            lineR = lrs[2];
        }

        // Ensure LineRenderers exist (assuming attached in Editor/MCP generation)
        if(manager != null) manager.OnTick += UpdateChart;
    }

    private void UpdateChart()
    {
        if (manager == null) return;
        
        histS.Enqueue(manager.countS);
        histI.Enqueue(manager.countI + manager.countQ); // Treat Q as Infectious logically for total 'cases' curve or separate if needed
        histR.Enqueue(manager.countR);
        
        if (histS.Count > maxHistory) histS.Dequeue();
        if (histI.Count > maxHistory) histI.Dequeue();
        if (histR.Count > maxHistory) histR.Dequeue();
        
        DrawCurve(lineS, histS, Color.green);
        DrawCurve(lineI, histI, Color.red);
        DrawCurve(lineR, histR, Color.gray);
    }
    
    private void DrawCurve(LineRenderer lr, Queue<int> hist, Color col)
    {
        if (lr == null) return;
        
        // Fix pink material issue
        if (lr.sharedMaterial == null || lr.sharedMaterial.name == "Default-Material")
        {
            lr.material = new Material(Shader.Find("Sprites/Default"));
        }

        lr.positionCount = hist.Count;
        lr.startColor = col;
        lr.endColor = col;
        lr.startWidth = 0.5f;
        lr.endWidth = 0.5f;
        
        var arr = hist.ToArray();
        
        // 400 is max agents assumption (or dynamically check manager count)
        float maxAgents = 400f; 
        
        for (int i = 0; i < arr.Length; i++)
        {
            float nx = (float)i / maxHistory;
            float ny = (float)arr[i] / maxAgents;
            
            Vector3 pos = transform.position + new Vector3(nx * width - width/2f, ny * height, 0);
            lr.SetPosition(i, pos);
        }
    }
}
