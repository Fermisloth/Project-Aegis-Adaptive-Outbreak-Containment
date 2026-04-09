using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(UIDocument))]
public class MainMenuController : MonoBehaviour
{
    private UIDocument uiDocument;
    private Button btnPlay;
    
    [Header("Scene Routing")]
    [Tooltip("Enter the exact name of your simulation scene.")]
    public string simulationSceneName = "Outbreak_Metaverse_Live";

    private void Start()
    {
        uiDocument = GetComponent<UIDocument>();
        if (uiDocument == null) return;
        
        var root = uiDocument.rootVisualElement;
        
        btnPlay = root.Q<Button>("btnPlay");
        
        if (btnPlay != null)
        {
            btnPlay.clicked += () => LoadSimulation();
        }
    }
    
    private void LoadSimulation()
    {
        SceneManager.LoadScene(simulationSceneName);
    }
}
