
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartGame : MonoBehaviour
{


    public Button startButton;
    public Button exitButton;
    public string sceneToLoad = "GameScene"; 

    void Start()
    {
        startButton.onClick.AddListener(StartGameFun);
        exitButton.onClick.AddListener(ExitGame);
    }

    void StartGameFun()
    {
        SceneManager.LoadScene(sceneToLoad);
    }

    void ExitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

}
