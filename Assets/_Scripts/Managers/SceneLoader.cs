using UnityEngine;
using UnityEngine.SceneManagement;
using HeartOfIron.Enums;
using System.Collections;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadScene(SceneName sceneName)
    {
        if (sceneName == SceneName.Loading)
        {
            return;
        } else if (sceneName == SceneName.CombatScene)
        {
            StartCoroutine(LoadSceneRoutine(sceneName));
        } else 
        {
            StartCoroutine(LoadSceneAsync(sceneName));
        }
        
    }

    private IEnumerator LoadSceneRoutine(SceneName sceneName)
    {
        // Wait for the Loading scene to fully activate before starting the
        // real load, otherwise the first progress events fire before
        // LoadingUIManager has subscribed to them.
        yield return SceneManager.LoadSceneAsync(SceneName.Loading.ToString());

        yield return LoadSceneAsync(sceneName);
    }

    private IEnumerator LoadSceneAsync(SceneName sceneName)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName.ToString());

        asyncLoad.allowSceneActivation = false;

        while (!asyncLoad.isDone)
        {
            float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f);
            // Debug.Log($"Loading progress: {progress * 100}%");

            // progress caps at 0.9 until activation is allowed, so isDone never
            // becomes true unless we flip this here once loading has finished.
            if (asyncLoad.progress >= 0.9f)
            {
                asyncLoad.allowSceneActivation = true;
            }

            yield return null;
        }
        // Debug.Log($"Finished loading scene: {sceneName}");
    }
}
