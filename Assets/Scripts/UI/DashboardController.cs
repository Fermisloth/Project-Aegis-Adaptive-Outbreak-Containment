using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(UIDocument))]
public class DashboardController : MonoBehaviour
{
    public SimulationManager manager;
    private UIDocument uiDocument;

    // UI Elements
    private Label lblBudget, lblHospital, lblPeak;
    private Label lblS, lblE, lblI, lblQ, lblR;
    private Label bigLblS, bigLblE, bigLblI, bigLblQ, bigLblR;
    private VisualElement barS, barE, barI, barQ, barR;
    private Button btnLockdown, btnVaccinate, btnReset;

    private void Start()
    {
        uiDocument = GetComponent<UIDocument>();
        if (uiDocument == null) return;

        var root = uiDocument.rootVisualElement;

        // Fetch Labels
        lblBudget = root.Q<Label>("lblBudget");
        lblHospital = root.Q<Label>("lblHospital");
        lblPeak = root.Q<Label>("lblPeak");
        
        lblS = root.Q<Label>("lblS");
        lblE = root.Q<Label>("lblE");
        lblI = root.Q<Label>("lblI");
        lblQ = root.Q<Label>("lblQ");
        lblR = root.Q<Label>("lblR");

        bigLblS = root.Q<Label>("bigLblS");
        bigLblE = root.Q<Label>("bigLblE");
        bigLblI = root.Q<Label>("bigLblI");
        bigLblQ = root.Q<Label>("bigLblQ");
        bigLblR = root.Q<Label>("bigLblR");
        
        // Fetch Graph Bars
        barS = root.Q<VisualElement>("barS");
        barE = root.Q<VisualElement>("barE");
        barI = root.Q<VisualElement>("barI");
        barQ = root.Q<VisualElement>("barQ");
        barR = root.Q<VisualElement>("barR");

        // Fetch Buttons
        btnLockdown = root.Q<Button>("btnLockdown");
        btnVaccinate = root.Q<Button>("btnVaccinate");
        btnReset = root.Q<Button>("btnReset");

        if (manager == null) manager = FindAnyObjectByType<SimulationManager>();

        if (manager != null)
        {
            manager.OnTick += UpdateTelemetry;
            
            // Setup Button Listeners
            if (btnLockdown != null) btnLockdown.clicked += () => manager.ExecuteAction(1);
            if (btnVaccinate != null) btnVaccinate.clicked += () => manager.ExecuteAction(2);
            if (btnReset != null) btnReset.clicked += () => SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    private void UpdateTelemetry()
    {
        if (manager == null) return;
        
        int maxHosp = (manager.config != null) ? manager.config.hospitalCapacity : 50;

        if (lblBudget != null) lblBudget.text = $"Budget: ${manager.remainingBudget}";
        if (lblHospital != null) lblHospital.text = $"Hospital: {manager.currentHospitalOccupancy} / {maxHosp}";
        if (lblPeak != null) lblPeak.text = $"Peak Cases: {manager.peakInfectious}";

        if (lblS != null) lblS.text = $"S: {manager.countS}";
        if (lblE != null) lblE.text = $"E: {manager.countE}";
        if (lblI != null) lblI.text = $"I: {manager.countI}";
        if (lblQ != null) lblQ.text = $"Q: {manager.countQ}";
        if (lblR != null) lblR.text = $"R: {manager.countR}";

        if (bigLblS != null) bigLblS.text = $"Susceptible: {manager.countS}";
        if (bigLblE != null) bigLblE.text = $"Exposed: {manager.countE}";
        if (bigLblI != null) bigLblI.text = $"Infectious: {manager.countI}";
        if (bigLblQ != null) bigLblQ.text = $"Quarantined: {manager.countQ}";
        if (bigLblR != null) bigLblR.text = $"Recovered: {manager.countR}";
        
        // Update Graph Bars
        float total = manager.model.agents.Count;
        if (total > 0)
        {
            if (barS != null) barS.style.width = new Length((manager.countS / total) * 100f, LengthUnit.Percent);
            if (barE != null) barE.style.width = new Length((manager.countE / total) * 100f, LengthUnit.Percent);
            if (barI != null) barI.style.width = new Length((manager.countI / total) * 100f, LengthUnit.Percent);
            if (barQ != null) barQ.style.width = new Length((manager.countQ / total) * 100f, LengthUnit.Percent);
            if (barR != null) barR.style.width = new Length((manager.countR / total) * 100f, LengthUnit.Percent);
        }
    }
}
