using UnityEngine;

public class ODEGraph : MonoBehaviour
{
    public SimulationConfig config;
    
    public int steps = 400; // Match panel width for 1:1 pixel mapping
    public float dt = 0.2f; 

    public int panelWidth = 400;
    public int panelHeight = 250;
    public float margin = 20f;

    private Texture2D graphTexture;
    private Color32[] pixels;

    void Start()
    {
        if (config == null) 
        {
            var manager = FindAnyObjectByType<SimulationManager>();
            if (manager != null) config = manager.config;
        }

        graphTexture = new Texture2D(panelWidth, panelHeight, TextureFormat.RGBA32, false);
        graphTexture.filterMode = FilterMode.Bilinear;
        pixels = new Color32[panelWidth * panelHeight];
    }

    void Update()
    {
        UpdateTexture();
    }

    void UpdateTexture()
    {
        if (config == null || graphTexture == null) return;

        float u = config.socialDistancing; 
        float nu = config.vaccinationRate;
        float beta = config.transmissionRate;
        float sigma = config.incubationRate;
        float gamma = config.recoveryRate;
        float delta = config.mortalityRate;

        float initialI = config.initialInfectedCount / 400f; 
        if (initialI <= 0) initialI = 0.01f;

        float S = 1.0f - initialI;
        float E = 0f;
        float I = initialI;
        float R = 0f;
        float V = 0f;
        float D = 0f;

        float[] arrS = new float[steps];
        float[] arrE = new float[steps];
        float[] arrI = new float[steps];
        float[] arrR = new float[steps];
        float[] arrV = new float[steps];
        float[] arrD = new float[steps];

        // Simulate
        for (int i = 0; i < steps; i++)
        {
            arrS[i] = S;
            arrE[i] = E;
            arrI[i] = I;
            arrR[i] = R;
            arrV[i] = V;
            arrD[i] = D;

            float dSdt = -(1f - u) * beta * S * I - nu * S;
            float dEdt = (1f - u) * beta * S * I - sigma * E;
            float dIdt = sigma * E - (gamma + delta) * I;
            float dRdt = gamma * I;
            float dVdt = nu * S;
            float dDdt = delta * I;

            S += dSdt * dt;
            E += dEdt * dt;
            I += dIdt * dt;
            R += dRdt * dt;
            V += dVdt * dt;
            D += dDdt * dt;
        }

        // Fill background
        Color32 bgColor = new Color32(25, 25, 35, 220);
        for (int i = 0; i < pixels.Length; i++) pixels[i] = bgColor;

        // Colors
        Color32 colS = new Color32(0, 255, 128, 150); // Green
        Color32 colE = new Color32(255, 204, 0, 150); // Yellow
        Color32 colI = new Color32(255, 0, 0, 150); // Red
        Color32 colR = new Color32(128, 128, 128, 150); // Gray
        Color32 colV = new Color32(0, 255, 255, 150); // Cyan
        Color32 colD = new Color32(0, 0, 0, 255); // Black
        Color32 colGrid = new Color32(200, 200, 200, 50);

        // Draw areas into pixels
        for (int x = 0; x < panelWidth; x++)
        {
            // Map x to step index
            int stepIdx = (int)(((float)x / panelWidth) * steps);
            if (stepIdx >= steps) stepIdx = steps - 1;

            int hS = (int)(arrS[stepIdx] * panelHeight);
            int hE = (int)(arrE[stepIdx] * panelHeight);
            int hI = (int)(arrI[stepIdx] * panelHeight);
            int hR = (int)(arrR[stepIdx] * panelHeight);
            int hV = (int)(arrV[stepIdx] * panelHeight);
            int hD = (int)(arrD[stepIdx] * panelHeight);

            // Simple stacking visualization in Z-order conceptually by overwriting
            for (int y = 0; y < panelHeight; y++)
            {
                int pIdx = y * panelWidth + x;
                
                // Draw grid lines
                if (x % 50 == 0 || y % 50 == 0) pixels[pIdx] = colGrid;

                if (y <= hS && hS > 0) pixels[pIdx] = Blend(pixels[pIdx], colS);
                if (y <= hE && hE > 0) pixels[pIdx] = Blend(pixels[pIdx], colE);
                if (y <= hR && hR > 0) pixels[pIdx] = Blend(pixels[pIdx], colR);
                if (y <= hV && hV > 0) pixels[pIdx] = Blend(pixels[pIdx], colV);
                if (y <= hI && hI > 0) pixels[pIdx] = Blend(pixels[pIdx], colI);
                if (y <= hD && hD > 0) pixels[pIdx] = Blend(pixels[pIdx], colD);
            }
            
            // Draw axis line base
            pixels[x] = new Color32(255, 255, 255, 255);
        }

        graphTexture.SetPixels32(pixels);
        graphTexture.Apply();
    }

    Color32 Blend(Color32 bg, Color32 fg)
    {
        float a = fg.a / 255f;
        return new Color32(
            (byte)(fg.r * a + bg.r * (1f - a)),
            (byte)(fg.g * a + bg.g * (1f - a)),
            (byte)(fg.b * a + bg.b * (1f - a)),
            255
        );
    }

    void OnGUI()
    {
        if (config == null || graphTexture == null) return;
        
        float screenW = Screen.width;
        float screenH = Screen.height;
        float startX = screenW - panelWidth - margin;
        float guiStartY = screenH - margin - panelHeight; 

        // Draw the natively generated Texture Graph
        GUI.DrawTexture(new Rect(startX, guiStartY, panelWidth, panelHeight), graphTexture, ScaleMode.StretchToFill);

        // Titles and Labels
        GUIStyle style = new GUIStyle();
        style.normal.textColor = Color.white;
        style.fontSize = 16;
        style.fontStyle = FontStyle.Bold;

        GUI.Label(new Rect(startX, guiStartY - 25, 300, 25), "Mathematical Prediction Demo", style);
        
        style.fontSize = 12;
        float legendX = startX + 10;
        float legendY = guiStartY + 10;

        style.normal.textColor = Color.green; GUI.Label(new Rect(legendX, legendY, 100, 20), "Susceptible", style); legendY += 15;
        style.normal.textColor = Color.yellow; GUI.Label(new Rect(legendX, legendY, 100, 20), "Exposed", style); legendY += 15;
        style.normal.textColor = Color.red; GUI.Label(new Rect(legendX, legendY, 100, 20), "Infectious", style); legendY += 15;
        style.normal.textColor = Color.gray; GUI.Label(new Rect(legendX, legendY, 100, 20), "Recovered", style); legendY += 15;
        style.normal.textColor = Color.cyan; GUI.Label(new Rect(legendX, legendY, 100, 20), "Vaccinated", style); legendY += 15;
        style.normal.textColor = Color.white; GUI.Label(new Rect(legendX, legendY, 100, 20), "Dead (Black)", style);
    }
}
