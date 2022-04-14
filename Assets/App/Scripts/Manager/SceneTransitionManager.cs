using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class SceneTransitionManager : AutoGenerateBaseManager<SceneTransitionManager>
{
    public readonly static string TitleSceneName = "Title";
    public readonly static string TutorialSceneNama = "Tutorial";

    public bool IsTransiting { get; private set; } = false;

    [SerializeField]
    private Image fadeImage = null;
    [SerializeField]
    private RawImage movieImage = null;
    [SerializeField]
    private VideoPlayer videoPlayer = null;
    [SerializeField]
    private float fadeTime = 1.0f;

    private float currentFadeTime = 0;

    /// <summary>
    /// マネージャーを生成する (ゲームロード時に自動で呼ばれる)
    /// </summary>
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void GenerateThisManager()
    {
        GenerateManager();
    }

    /// <summary>
    /// 特定の名前のシーンに遷移させる
    /// </summary>
    /// <param name="sceneName"></param>
    /// <param name="loadWaitingTime"></param>
    public void TransitionByName(string sceneName, TransitionType transitionType, float fadeOutTime = 1.0f, float fadeInTime = 1.0f)
    {
        StartCoroutine(TransitionScene(sceneName, transitionType, fadeOutTime, fadeInTime));
    }

    /// <summary>
    /// 特定の名前のシーンに動画を流して遷移させる
    /// </summary>
    /// <param name="sceneName"></param>
    /// <param name="loadWaitingTime"></param>
    public void TransitionWithMovieByName(string sceneName, string movieAssetAddress, float fadeOutTime = 1.0f, float fadeInTime = 1.0f)
    {
        StartCoroutine(TransitionSceneWithMovie(sceneName, movieAssetAddress, fadeOutTime, fadeInTime));
    }

    /// <summary>
    /// シーン遷移をさせるコルーチン
    /// </summary>
    /// <param name="sceneName"></param>
    /// <param name="transitionType"></param>
    /// <returns></returns>
    private IEnumerator TransitionScene(string sceneName, TransitionType transitionType, float fadeOutTime, float fadeInTime)
    {
        IsTransiting = true;
        // フェードアウトさせる
        yield return StartCoroutine(FadeOut(fadeOutTime));
        // シーンをロードする
        AsyncOperation loadAsync = SceneManager.LoadSceneAsync(sceneName);
        do
        {
            yield return null;
        }
        while (!loadAsync.isDone);

        // フェードインさせる
        yield return StartCoroutine(FadeIn(fadeInTime));

        IsTransiting = false;
    }

    /// <summary>
    /// 動画を再生してシーン遷移をさせるコルーチン
    /// </summary>
    /// <param name="sceneName"></param>
    /// <param name="transitionType"></param>
    /// <returns></returns>
    private IEnumerator TransitionSceneWithMovie(string sceneName, string movieAssetAddress, float fadeOutTime, float fadeInTime)
    {
        IsTransiting = true;
        // フェードアウトさせる
        yield return StartCoroutine(FadeOut(fadeOutTime));
        // 動画を読み込む
        var handle = Addressables.LoadAssetAsync<VideoClip>(movieAssetAddress);
        yield return handle;
        // 読み込みが完了したら再生する
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            movieImage.enabled = true;
            yield return StartCoroutine(WaitForPlayedMovie(handle.Result));
            movieImage.enabled = false;
        }

        // シーンをロードする
        AsyncOperation loadAsync = SceneManager.LoadSceneAsync(sceneName);
        while (!loadAsync.isDone)
        {
            yield return null;
        }

        // フェードインさせる
        yield return StartCoroutine(FadeIn(fadeInTime));

        IsTransiting = false;
    }

    /// <summary>
    /// フェードアウトさせるコルーチン
    /// </summary>
    /// <returns></returns>
    private IEnumerator FadeOut(float fadeTime)
    {
        currentFadeTime = 0;
        while(currentFadeTime < fadeTime)
        {
            currentFadeTime = Mathf.Clamp(currentFadeTime + Time.unscaledDeltaTime, 0, fadeTime);
            SetAlphaImage(currentFadeTime / fadeTime);

            yield return null;
        }
    }

    /// <summary>
    /// フェードインさせるコルーチン
    /// </summary>
    /// <returns></returns>
    private IEnumerator FadeIn(float fadeTime)
    {
        currentFadeTime = 0;
        while (currentFadeTime < fadeTime)
        {
            currentFadeTime = Mathf.Clamp(currentFadeTime + Time.unscaledDeltaTime, 0, fadeTime);
            SetAlphaImage(1 - (currentFadeTime / fadeTime));

            yield return null;
        }
    }

    /// <summary>
    /// 動画を再生し、終了するまで待つ
    /// </summary>
    /// <param name="videoClip"></param>
    /// <returns></returns>
    private IEnumerator WaitForPlayedMovie(VideoClip videoClip)
    {
        videoPlayer.clip = videoClip;
        videoPlayer.Play();

        // videoPlayerのframeは0からカウントするため -1 する
        ulong videoFrameLenght = videoClip.frameCount - 1;
        // 動画が最後まで再生されるまで待つ
        while ((ulong)videoPlayer.frame != videoFrameLenght)
        {
            yield return null;
        }

        videoPlayer.clip = null;
    }

    /// <summary>
    /// フェード用の画像のアルファ値をセットする
    /// </summary>
    /// <param name="alpha"></param>
    private void SetAlphaImage(float alpha)
    {
        Color fadeColor = Color.black;
        fadeColor.a = alpha;

        fadeImage.color = fadeColor;
    }
}

public enum TransitionType
{
    FadeInOut,
}