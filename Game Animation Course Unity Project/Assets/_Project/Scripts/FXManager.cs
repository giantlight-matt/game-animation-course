using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Serialization;
using UnityEngine.Events;

public class FXManager : Singleton<FXManager>
{
    [Header("Solids/Overlays")]
    [FormerlySerializedAs("overlaySolid")]
    public Image foregroundSolid;
    public Image backgroundSolid;
    public Image foregroundFlashSolid;
    public Image backgroundFlashSolid;

    private Coroutine _fadeCoroutine;

    private Camera _cameraMain;

	private void Awake()
	{
        _cameraMain = Camera.main;
	}

    public void JumpToBlack(){
        foregroundSolid.color = _cameraMain.backgroundColor;
    }

    public void JumpToClear(){
        var color = _cameraMain.backgroundColor;
        color.a = 0;
        foregroundSolid.color = color;
    }

    #region Fades
    public void FadeIn(float duration = 1f){
        if(_fadeCoroutine != null){
            StopCoroutine(_fadeCoroutine);
        }
        _fadeCoroutine = StartCoroutine(FadeInCoroutine(duration));
    }

    IEnumerator FadeInCoroutine(float duration = 1f){
        var startAlpha = foregroundSolid.color.a;
        var pct = startAlpha;

        DOTween.Kill(foregroundSolid);
        yield return foregroundSolid.DOFade(0f, pct * duration).WaitForCompletion();
    }

    public void FadeOut(float duration = 1f){
        if(_fadeCoroutine != null){
            StopCoroutine(_fadeCoroutine);
        }
        _fadeCoroutine = StartCoroutine(FadeOutCoroutine(duration));
    }

    IEnumerator FadeOutCoroutine(float duration = 1f){
        var startAlpha = foregroundSolid.color.a;
        var pct = 1f-startAlpha;

        DOTween.Kill(foregroundSolid);
        yield return foregroundSolid.DOFade(1f, pct * duration).WaitForCompletion();
    }
    #endregion

    #region Flashes
    Tween _currentForegroundFlashTween;
    Tween _currentBackgroundFlashTween;
    const float DefaultFlashIntensity = .5f;
    const float DefaultFlashFadeDuration = .5f;

    public void DoFlash(){
        DoFlash(Color.white, foregroundFlashSolid, DefaultFlashIntensity, DefaultFlashFadeDuration);
    }
    public void DoFlash(Color color){
        DoFlash(color, foregroundFlashSolid, DefaultFlashIntensity, DefaultFlashFadeDuration);
    }
    public void DoFlash(float intensity){
        DoFlash(Color.white, foregroundFlashSolid, intensity, DefaultFlashFadeDuration);
    }
    public void DoFlash(Color color, float intensity){
        DoFlash(color, foregroundFlashSolid, intensity, DefaultFlashFadeDuration);
    }

    public void DoBackgroundFlash(){
        DoFlash(Color.white, backgroundFlashSolid, DefaultFlashIntensity, DefaultFlashFadeDuration);
    }
    public void DoBackgroundFlash(Color color){
        DoFlash(color, backgroundFlashSolid, DefaultFlashIntensity, DefaultFlashFadeDuration);
    }
    public void DoBackgroundFlash(float intensity){
        DoFlash(Color.white, backgroundFlashSolid, intensity, DefaultFlashFadeDuration);
    }
    public void DoBackgroundFlash(Color color, float intensity){
        DoFlash(color, backgroundFlashSolid, intensity, DefaultFlashFadeDuration);
    }


    public void DoFlash(Color color, Image solid = null, float intensity = DefaultFlashIntensity, float flashFadeDuration = DefaultFlashFadeDuration){
        DOTween.Kill(solid);

        color.a = intensity;
        solid.color = color;

        var clearColor = color;
        clearColor.a = 0;

        solid.DOColor(clearColor, flashFadeDuration);
    }
    #endregion

    #region Camera Shake
    const float DefaultShakeIntensity = .1f;
    
    private float shakeDecay = .2f;
    private float shakeSpeed = 70f;

    private float _totalShakeIntensity;
    private float _totalShakeDuration;

    public event UnityAction<Vector3> OnShake;

    public void DoScreenShake(float intensity = DefaultShakeIntensity){
        StartCoroutine(ScreenShakeCoroutine(intensity));
    }

    Tween _currentShakeTween;
    private IEnumerator ScreenShakeCoroutine(float intensity){
        if(intensity > _totalShakeIntensity){
            _totalShakeIntensity = intensity;
        }

        // TODO: Improvement
        // Would prefer to use tweens here, but need to think of a way
        // to prevent smaller shakes from stopping bigger shakes, or
        // smaller shakes preventing bigger shakes.
        while(_totalShakeIntensity > 0){
            var time = Time.timeSinceLevelLoad * shakeSpeed;

            float x = (Mathf.PerlinNoise(1, time) - 0.5f) * 2f;
            float y = (Mathf.PerlinNoise(10, time) - 0.5f) * 2f;
            float z = (Mathf.PerlinNoise(100, time) - 0.5f) * 2f;

            var shakeOffset = new Vector3(x,y,z) * _totalShakeIntensity;
            OnShake?.Invoke(shakeOffset);

            _totalShakeIntensity -= shakeDecay * Time.deltaTime;
            yield return null;
        }
    }

    #endregion


    #region Combined
    public void OnGameOver(){
        var transBlack = Color.black;
        DoFlash(Color.black, foregroundFlashSolid, .05f, .5f);
        DoScreenShake();
    }

    public void OnWin(){
        var lightGold = Color.Lerp(Color.white, Color.yellow, .23f);
        DoFlash(lightGold, foregroundFlashSolid, .05f, 1f);
    }

    public void OnPowerupGet(){
        var lightGold = Color.Lerp(Color.white, Color.yellow, .5f);
        DoFlash(lightGold, backgroundFlashSolid, .25f, 4f);
    }
    #endregion

}