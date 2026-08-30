using UnityEngine;
using UnityEngine.SceneManagement; // Panggil namespace ini

public class LevelManager : MonoBehaviour
{
    public void LoadToScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
