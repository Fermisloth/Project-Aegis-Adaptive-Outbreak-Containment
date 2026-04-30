using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using System.Collections;

[RequireComponent(typeof(UIDocument))]
public class MainMenuController : MonoBehaviour
{
    private UIDocument uiDocument;
    private Button btnPlay;

    // Loading overlay elements
    private VisualElement loadingOverlay;
    private VisualElement progressBar;
    private Label loadingHint;

    // Spinner dots
    private VisualElement[] dots = new VisualElement[5];

    [Header("Scene Routing")]
    [Tooltip("Enter the exact name of your simulation scene.")]
    public string simulationSceneName = "Outbreak_Metaverse_Live";

    private static readonly string[] hints = new string[]
    {
        "Loading city layout…",
        "Spawning agent population…",
        "Initialising SEIQR model…",
        "Calibrating infection rates…",
        "Preparing containment protocols…",
        "Simulation ready."
    };

    private void Start()
    {
        uiDocument = GetComponent<UIDocument>();
        if (uiDocument == null) return;

        var root = uiDocument.rootVisualElement;

        btnPlay        = root.Q<Button>("btnPlay");
        loadingOverlay = root.Q<VisualElement>("LoadingOverlay");
        progressBar    = root.Q<VisualElement>("progressBar");
        loadingHint    = root.Q<Label>("loadingHint");

        for (int i = 0; i < dots.Length; i++)
            dots[i] = root.Q<VisualElement>($"dot{i}");

        if (btnPlay != null)
            btnPlay.clicked += OnPlayClicked;
    }

    private void OnPlayClicked()
    {
        // Disable button to prevent double-click
        if (btnPlay != null) btnPlay.SetEnabled(false);
        StartCoroutine(PlayLoadingAnimation());
    }

    private IEnumerator PlayLoadingAnimation()
    {
        // Show overlay
        if (loadingOverlay != null)
            loadingOverlay.style.display = DisplayStyle.Flex;

        // Start async load in background
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(simulationSceneName);
        asyncLoad.allowSceneActivation = false;

        float elapsed = 0f;
        float fakeLoadTime = 2.8f; // animate the bar for this many seconds before activating
        int dotIndex = 0;
        int hintIndex = 0;
        float nextHint = 0f;
        float nextDot  = 0f;

        while (elapsed < fakeLoadTime || asyncLoad.progress < 0.9f)
        {
            elapsed += Time.deltaTime;

            // Progress bar (blend fake progress with real)
            float fakeProgress = Mathf.Clamp01(elapsed / fakeLoadTime);
            float realProgress = asyncLoad.progress / 0.9f;
            float displayProgress = Mathf.Max(fakeProgress, realProgress) * 100f;

            if (progressBar != null)
                progressBar.style.width = new Length(displayProgress, LengthUnit.Percent);

            // Cycle loading hints
            if (elapsed > nextHint)
            {
                nextHint = elapsed + fakeLoadTime / hints.Length;
                if (loadingHint != null && hintIndex < hints.Length)
                    loadingHint.text = hints[hintIndex++];
            }

            // Animate spinner dots (sequential pulse)
            if (elapsed > nextDot)
            {
                nextDot = elapsed + 0.18f;
                for (int i = 0; i < dots.Length; i++)
                {
                    if (dots[i] == null) continue;
                    dots[i].style.opacity = (i == dotIndex % dots.Length) ? 1f : 0.2f;
                }
                dotIndex++;
            }

            yield return null;
        }

        // Fill to 100%
        if (progressBar != null)
            progressBar.style.width = new Length(100f, LengthUnit.Percent);
        if (loadingHint != null)
            loadingHint.text = "Simulation ready.";

        // Brief pause so user sees 100%
        yield return new WaitForSeconds(0.35f);

        // Activate scene
        asyncLoad.allowSceneActivation = true;
    }
}
