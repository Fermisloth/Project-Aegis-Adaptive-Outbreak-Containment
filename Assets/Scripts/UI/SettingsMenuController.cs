using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;

[RequireComponent(typeof(UIDocument))]
public class SettingsMenuController : MonoBehaviour
{
    private VisualElement overlay;
    private Slider sliderContagious;
    private Slider sliderLethal;
    private Slider sliderMale;
    private Slider sliderChild;
    private Button btnApply;
    private Button btnClose;

    private void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        if (root == null) return;
        
        overlay = root.Q<VisualElement>("settings-overlay");
        sliderContagious = root.Q<Slider>("slider-contagious");
        sliderLethal = root.Q<Slider>("slider-lethal");
        sliderMale = root.Q<Slider>("slider-male");
        sliderChild = root.Q<Slider>("slider-child");
        
        btnApply = root.Q<Button>("btn-apply");
        btnClose = root.Q<Button>("btn-close");

        if (btnApply != null) btnApply.clicked += ApplyAndRespawn;
        if (btnClose != null) btnClose.clicked += ToggleMenu;

        // Initialize values
        if (SimulationManager.Instance != null && SimulationManager.Instance.config != null)
        {
            sliderContagious.value = SimulationManager.Instance.config.transmissionRate;
            sliderLethal.value = SimulationManager.Instance.config.mortalityRate;
        }

        var spawner = Object.FindFirstObjectByType<AgentGridSpawner>();
        if (spawner != null)
        {
            sliderMale.value = spawner.maleFraction;
            sliderChild.value = spawner.childFraction;
        }

        // Ensure it is hidden by default
        if (overlay != null)
        {
            overlay.AddToClassList("hidden");
            overlay.style.display = DisplayStyle.None;
        }
    }

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            Debug.Log("ESC pressed!");
            ToggleMenu();
        }
    }

    private void ToggleMenu()
    {
        if (overlay == null)
        {
            Debug.LogWarning("SettingsMenuController: Overlay is null!");
            // Attempt to find it again
            var root = GetComponent<UIDocument>().rootVisualElement;
            if (root != null) overlay = root.Q<VisualElement>("settings-overlay");
            if (overlay == null) return;
        }
        
        bool isHidden = overlay.ClassListContains("hidden");
        Debug.Log("ToggleMenu called. isHidden = " + isHidden);
        
        if (isHidden)
        {
            // Update sliders before showing just in case
            if (SimulationManager.Instance != null && SimulationManager.Instance.config != null)
            {
                sliderContagious.value = SimulationManager.Instance.config.transmissionRate;
                sliderLethal.value = SimulationManager.Instance.config.mortalityRate;
            }

            var spawner = Object.FindFirstObjectByType<AgentGridSpawner>();
            if (spawner != null)
            {
                sliderMale.value = spawner.maleFraction;
                sliderChild.value = spawner.childFraction;
            }

            overlay.RemoveFromClassList("hidden");
            overlay.style.display = DisplayStyle.Flex;
            Time.timeScale = 0f; // Pause sim
        }
        else
        {
            overlay.AddToClassList("hidden");
            overlay.style.display = DisplayStyle.None;
            Time.timeScale = 1f; // Resume sim
        }
    }

    private void ApplyAndRespawn()
    {
        if (SimulationManager.Instance != null && SimulationManager.Instance.config != null)
        {
            SimulationManager.Instance.config.transmissionRate = sliderContagious.value;
            SimulationManager.Instance.config.mortalityRate = sliderLethal.value;
        }

        var spawner = Object.FindFirstObjectByType<AgentGridSpawner>();
        if (spawner != null)
        {
            spawner.Respawn(sliderMale.value, sliderChild.value);
        }

        ToggleMenu();
    }
}
