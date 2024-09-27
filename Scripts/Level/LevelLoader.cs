using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    [SerializeField] private SceneReference[] scenesToPreload = null;

    private void Awake()
    {
        foreach (var scene in scenesToPreload)
        {
            PreloadScene(scene);
        }
    }

    private IEnumerator PreloadScene(SceneReference _scene)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(_scene.ScenePath, LoadSceneMode.Additive);
        
        while (!asyncLoad.isDone)
            yield return null;
    }
}
