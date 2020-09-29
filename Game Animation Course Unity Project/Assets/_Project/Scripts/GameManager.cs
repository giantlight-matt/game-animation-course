using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    public IEnumerator Start(){
        FXManager.Instance.JumpToBlack();
        yield return new WaitForSeconds(1f);
        FXManager.Instance.FadeIn(1f);
    }

    public void TransitionToScene(string newSceneName){
        StartCoroutine(TransitionToSceneCoroutine(newSceneName));
    }

    public IEnumerator TransitionToSceneCoroutine(string newSceneName){
        float transitionDuration = 1f;

        FXManager.Instance.FadeOut(transitionDuration);
        yield return new WaitForSeconds(transitionDuration);

        yield return SceneManager.LoadSceneAsync(newSceneName, LoadSceneMode.Single);
    }

    public void HandleWin(string nextSceneName){
        StartCoroutine(HandleWinCoroutine(nextSceneName));
    }

    public IEnumerator HandleWinCoroutine(string nextSceneName){
        yield return new WaitForSeconds(2f);
        // reload to current scene
        if(string.IsNullOrEmpty(nextSceneName)){
            TransitionToScene(SceneManager.GetActiveScene().name);
        }else{
            TransitionToScene(nextSceneName);
        }
    }

    public void HandleLose(){
        StartCoroutine(HandleLoseCoroutine());
    }

    public IEnumerator HandleLoseCoroutine(){
        yield return new WaitForSeconds(5f);
        // reload to current scene
        TransitionToScene(SceneManager.GetActiveScene().name);

    }


}
