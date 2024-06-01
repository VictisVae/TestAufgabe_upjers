using System;
using CodeBase.Utilities;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CodeBase.Infrastructure.GameBoot.Editor {
  [InitializeOnLoad]
  public static class FirstSceneLoader {
    private static void OnPlayModeStateChanged(PlayModeStateChange state) {
      if (EditorApplication.isPlaying == false && EditorApplication.isPlayingOrWillChangePlaymode) {
        EditorPrefs.SetString(nameof(FirstSceneLoader), SceneManager.GetActiveScene().path);

        if (SceneManager.GetActiveScene().buildIndex == 0) {
          return;
        }

        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) {
          TryLoadScene(Constants.Boot.FirstScenePath);
        } else {
          EditorApplication.isPlaying = false;
        }
      }

      if (EditorApplication.isPlaying == false && EditorApplication.isPlayingOrWillChangePlaymode == false) {
        TryLoadScene(EditorPrefs.GetString(nameof(FirstSceneLoader)));
      }
    }

    private static void TryLoadScene(string sceneName) {
      try {
        EditorSceneManager.OpenScene(sceneName);
      } catch (Exception) {
        EditorApplication.isPlaying = false;
        Debug.LogError($"Cannot log first scene {sceneName}");
      }
    }
    
    static FirstSceneLoader() =>
      EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
  }
}