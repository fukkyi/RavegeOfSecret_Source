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
    /// �}�l�[�W���[�𐶐����� (�Q�[�����[�h���Ɏ����ŌĂ΂��)
    /// </summary>
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void GenerateThisManager()
    {
        GenerateManager();
    }

    /// <summary>
    /// ����̖��O�̃V�[���ɑJ�ڂ�����
    /// </summary>
    /// <param name="sceneName"></param>
    /// <param name="loadWaitingTime"></param>
    public void TransitionByName(string sceneName, TransitionType transitionType, float fadeOutTime = 1.0f, float fadeInTime = 1.0f)
    {
        StartCoroutine(TransitionScene(sceneName, transitionType, fadeOutTime, fadeInTime));
    }

    /// <summary>
    /// ����̖��O�̃V�[���ɓ���𗬂��đJ�ڂ�����
    /// </summary>
    /// <param name="sceneName"></param>
    /// <param name="loadWaitingTime"></param>
    public void TransitionWithMovieByName(string sceneName, string movieAssetAddress, float fadeOutTime = 1.0f, float fadeInTime = 1.0f)
    {
        StartCoroutine(TransitionSceneWithMovie(sceneName, movieAssetAddress, fadeOutTime, fadeInTime));
    }

    /// <summary>
    /// �V�[���J�ڂ�������R���[�`��
    /// </summary>
    /// <param name="sceneName"></param>
    /// <param name="transitionType"></param>
    /// <returns></returns>
    private IEnumerator TransitionScene(string sceneName, TransitionType transitionType, float fadeOutTime, float fadeInTime)
    {
        IsTransiting = true;
        // �t�F�[�h�A�E�g������
        yield return StartCoroutine(FadeOut(fadeOutTime));
        // �V�[�������[�h����
        AsyncOperation loadAsync = SceneManager.LoadSceneAsync(sceneName);
        do
        {
            yield return null;
        }
        while (!loadAsync.isDone);

        // �t�F�[�h�C��������
        yield return StartCoroutine(FadeIn(fadeInTime));

        IsTransiting = false;
    }

    /// <summary>
    /// ������Đ����ăV�[���J�ڂ�������R���[�`��
    /// </summary>
    /// <param name="sceneName"></param>
    /// <param name="transitionType"></param>
    /// <returns></returns>
    private IEnumerator TransitionSceneWithMovie(string sceneName, string movieAssetAddress, float fadeOutTime, float fadeInTime)
    {
        IsTransiting = true;
        // �t�F�[�h�A�E�g������
        yield return StartCoroutine(FadeOut(fadeOutTime));
        // �����ǂݍ���
        var handle = Addressables.LoadAssetAsync<VideoClip>(movieAssetAddress);
        yield return handle;
        // �ǂݍ��݂�����������Đ�����
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            movieImage.enabled = true;
            yield return StartCoroutine(WaitForPlayedMovie(handle.Result));
            movieImage.enabled = false;
        }

        // �V�[�������[�h����
        AsyncOperation loadAsync = SceneManager.LoadSceneAsync(sceneName);
        while (!loadAsync.isDone)
        {
            yield return null;
        }

        // �t�F�[�h�C��������
        yield return StartCoroutine(FadeIn(fadeInTime));

        IsTransiting = false;
    }

    /// <summary>
    /// �t�F�[�h�A�E�g������R���[�`��
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
    /// �t�F�[�h�C��������R���[�`��
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
    /// ������Đ����A�I������܂ő҂�
    /// </summary>
    /// <param name="videoClip"></param>
    /// <returns></returns>
    private IEnumerator WaitForPlayedMovie(VideoClip videoClip)
    {
        videoPlayer.clip = videoClip;
        videoPlayer.Play();

        // videoPlayer��frame��0����J�E���g���邽�� -1 ����
        ulong videoFrameLenght = videoClip.frameCount - 1;
        // ���悪�Ō�܂ōĐ������܂ő҂�
        while ((ulong)videoPlayer.frame != videoFrameLenght)
        {
            yield return null;
        }

        videoPlayer.clip = null;
    }

    /// <summary>
    /// �t�F�[�h�p�̉摜�̃A���t�@�l���Z�b�g����
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