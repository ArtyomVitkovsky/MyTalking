using System.Collections;
using Modules.MainModule.Scripts.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace Modules.MainModule.Scripts
{
    public class SceneLoader : MonoBehaviour
    {
        private UIManager uiManager;
        [Inject]
        private void Construct(UIManager uiManager)
        {
            this.uiManager = uiManager;
        }
        private void Start()
        {
            LoadGameScene();
        }

        private void LoadGameScene()
        {
            GameSettings.IS_PAUSED = true;
            StartCoroutine(LoadGameSceneCoroutine());
        }

        private IEnumerator LoadGameSceneCoroutine()
        {
            yield return StartCoroutine(LoadSceneCoroutine("GameScene", LoadSceneMode.Additive));
            uiManager.ShowGameScreen();
        }

        public void LoadScene(string sceneName, LoadSceneMode loadSceneMode)
        {
            StartCoroutine(LoadSceneCoroutine(sceneName, loadSceneMode));
        }
        
        public void ReloadGameScene()
        {
            StartCoroutine(ReloadGameSceneCoroutine("GameScene"));
        }
        
        public IEnumerator LoadSceneCoroutine(string sceneName, LoadSceneMode loadSceneMode)
        {
            if (SceneManager.GetSceneByName(sceneName).isLoaded) yield break;
            
            uiManager.ShowLoadingScreen();
            yield return SceneManager.LoadSceneAsync(sceneName, loadSceneMode);
            var loadedScene = SceneManager.GetSceneByName(sceneName);
            SceneManager.SetActiveScene(loadedScene);
        }
        
        public IEnumerator UnloadSceneCoroutine(string sceneName)
        {
            yield return SceneManager.UnloadSceneAsync(sceneName);
        }

        public IEnumerator ReloadGameSceneCoroutine(string sceneName)
        {
            yield return StartCoroutine(UnloadSceneCoroutine(sceneName));
            yield return StartCoroutine(LoadSceneCoroutine(sceneName, LoadSceneMode.Additive));
            uiManager.ShowGameScreen();
        }
    }
}