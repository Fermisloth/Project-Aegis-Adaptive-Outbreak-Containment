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
    private Button btnLockdown, btnVaccinate, btnQuarantine, btnReset;

    // Lockdown duration buttons
    private Button btnDuration5, btnDuration10, btnDuration20, btnDuration30;
    private Button activeDurationBtn;

    // Overlay message
    private VisualElement overlayMessage;
    private Label overlayText;
    private Coroutine overlayCoroutine;

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
        btnQuarantine = root.Q<Button>("btnQuarantine");
        btnReset = root.Q<Button>("btnReset");

        // Fetch Lockdown Duration Buttons
        btnDuration5 = root.Q<Button>("btnDuration5");
        btnDuration10 = root.Q<Button>("btnDuration10");
        btnDuration20 = root.Q<Button>("btnDuration20");
        btnDuration30 = root.Q<Button>("btnDuration30");

        // Fetch Overlay
        overlayMessage = root.Q<VisualElement>("OverlayMessage");
        overlayText = root.Q<Label>("overlayText");
        
        SetupSettingsMenu(root);

        if (manager == null) manager = FindAnyObjectByType<SimulationManager>();

        if (manager != null)
        {
            manager.OnTick += UpdateTelemetry;
            manager.OnActionMessage += ShowOverlayMessage;
            
            // Setup Button Listeners
            if (btnLockdown != null) btnLockdown.clicked += () => manager.ExecuteAction(1);
            if (btnVaccinate != null) btnVaccinate.clicked += () => manager.ExecuteAction(2);
            if (btnQuarantine != null) btnQuarantine.clicked += () => manager.ExecuteAction(3);
            if (btnReset != null) btnReset.clicked += () => SceneManager.LoadScene(SceneManager.GetActiveScene().name);

            // Setup Duration Button Listeners
            SetupDurationButton(btnDuration5, 5f);
            SetupDurationButton(btnDuration10, 10f);
            SetupDurationButton(btnDuration20, 20f);
            SetupDurationButton(btnDuration30, 30f);

            // Default: 5s is active
            activeDurationBtn = btnDuration5;
            manager.selectedLockdownDuration = 5f;
        }
    }

    private void SetupDurationButton(Button btn, float duration)
    {
        if (btn == null) return;
        btn.clicked += () =>
        {
            // Remove active class from previous
            if (activeDurationBtn != null)
                activeDurationBtn.RemoveFromClassList("duration-btn-active");

            // Set new active
            btn.AddToClassList("duration-btn-active");
            activeDurationBtn = btn;

            // Update manager
            if (manager != null)
                manager.selectedLockdownDuration = duration;
        };
    }

    private void ShowOverlayMessage(string message)
    {
        if (overlayMessage == null || overlayText == null) return;

        overlayText.text = message;
        overlayMessage.style.display = DisplayStyle.Flex;
        overlayMessage.style.opacity = 1f;

        // Cancel previous fade if any
        if (overlayCoroutine != null)
            StopCoroutine(overlayCoroutine);

        overlayCoroutine = StartCoroutine(FadeOutOverlay());
    }

    private IEnumerator FadeOutOverlay()
    {
        // Show for 1.5 seconds
        yield return new WaitForSeconds(1.5f);

        // Fade out over 0.5 seconds
        float fadeDuration = 0.5f;
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            if (overlayMessage != null)
                overlayMessage.style.opacity = alpha;
            yield return null;
        }

        // Hide completely
        if (overlayMessage != null)
            overlayMessage.style.display = DisplayStyle.None;

        overlayCoroutine = null;
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

    private void OnDestroy()
    {
        if (manager != null)
        {
            manager.OnTick -= UpdateTelemetry;
            manager.OnActionMessage -= ShowOverlayMessage;
        }
    }

    // === Settings Menu Logic ===
    private VisualElement settingsOverlay;
    private Slider sliderContagious;
    private Slider sliderLethal;
    private Slider sliderMale;
    private Slider sliderChild;
    private SliderInt sliderPopulation;
    private Button btnApplySettings;
    private Button btnCloseSettings;

    private void SetupSettingsMenu(VisualElement root)
    {
        settingsOverlay = root.Q<VisualElement>("settings-overlay");
        sliderContagious = root.Q<Slider>("slider-contagious");
        sliderLethal = root.Q<Slider>("slider-lethal");
        sliderMale = root.Q<Slider>("slider-male");
        sliderChild = root.Q<Slider>("slider-child");
        sliderPopulation = root.Q<SliderInt>("slider-population");
        
        btnApplySettings = root.Q<Button>("btn-apply");
        btnCloseSettings = root.Q<Button>("btn-close");

        if (btnApplySettings != null) btnApplySettings.clicked += ApplyAndRespawn;
        if (btnCloseSettings != null) btnCloseSettings.clicked += ToggleMenu;

        if (settingsOverlay != null)
        {
            settingsOverlay.AddToClassList("hidden");
            settingsOverlay.style.display = DisplayStyle.None;
        }
    }

    private void Update()
    {
        if (UnityEngine.InputSystem.Keyboard.current != null && UnityEngine.InputSystem.Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            ToggleMenu();
        }
    }

    private void ToggleMenu()
    {
        if (settingsOverlay == null) return;
        
        bool isHidden = settingsOverlay.ClassListContains("hidden");
        
        if (isHidden)
        {
            if (manager != null && manager.config != null)
            {
                sliderContagious.value = manager.config.transmissionRate;
                sliderLethal.value = manager.config.mortalityRate;
            }

            var spawner = Object.FindFirstObjectByType<AgentGridSpawner>();
            if (spawner != null)
            {
                if (sliderPopulation != null) sliderPopulation.value = spawner.populationSize;
                sliderMale.value = spawner.maleFraction;
                sliderChild.value = spawner.childFraction;
            }

            settingsOverlay.RemoveFromClassList("hidden");
            settingsOverlay.style.display = DisplayStyle.Flex;
            Time.timeScale = 0f; // Pause sim
        }
        else
        {
            settingsOverlay.AddToClassList("hidden");
            settingsOverlay.style.display = DisplayStyle.None;
            Time.timeScale = 1f; // Resume sim
        }
    }

    private void ApplyAndRespawn()
    {
        if (manager != null && manager.config != null)
        {
            manager.config.transmissionRate = sliderContagious.value;
            manager.config.mortalityRate = sliderLethal.value;
        }

        var spawner = Object.FindFirstObjectByType<AgentGridSpawner>();
        if (spawner != null)
        {
            int pop = sliderPopulation != null ? sliderPopulation.value : spawner.populationSize;
            spawner.Respawn(pop, sliderMale.value, sliderChild.value);
        }

        ToggleMenu();
    }
}
