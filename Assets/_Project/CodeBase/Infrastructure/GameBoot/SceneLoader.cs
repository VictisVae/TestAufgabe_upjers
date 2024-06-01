using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CodeBase.Infrastructure.GameBoot {
  public class SceneLoader {
    public SceneLoader(ICoroutineHandler coroutineHandler) => CoroutineHandler = coroutineHandler;
    public void Load(string sceneName, Action onLoaded = null) => CoroutineHandler.StartCoroutine(LoadScene(sceneName, onLoaded));

    private IEnumerator LoadScene(string nextScene, Action onLoaded = null) {
      if (SceneManager.GetActiveScene().name.Equals(nextScene)) {
        onLoaded?.Invoke();
        yield break;
      }

      AsyncOperation waitNextScene = SceneManager.LoadSceneAsync(nextScene);

      while (waitNextScene.isDone == false) {
        yield return null;
      }

      onLoaded?.Invoke();
    }

    public ICoroutineHandler CoroutineHandler { get; }
  }
}