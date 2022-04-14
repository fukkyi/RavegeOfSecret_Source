using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ResultController : MonoBehaviour
{
    [SerializeField]
    private Animator resultAnim = null;
    [SerializeField]
    private Animator finishAnim = null;
    [SerializeField]
    private Image rankImage = null;
    [SerializeField]
    private Text clearTimeText = null;
    [SerializeField]
    private Text damageCountText = null;
    [SerializeField]
    private Text clearScoreText = null;
    [SerializeField]
    private Sprite rankSpriteC = null;
    [SerializeField]
    private Sprite rankSpriteB = null;
    [SerializeField]
    private Sprite rankSpriteA = null;
    [SerializeField]
    private Sprite rankSPriteS = null;

    // Start is called before the first frame update
    void Start()
    {
        if (ScoreManager.Instance != null)
        {
            SetRankSprite(ScoreManager.Instance.GetPlayRank());
            SetClearTime(ScoreManager.Instance.GetPlayRankForTimeSpan());
            SetDamageCount(ScoreManager.Instance.DamegeCount);
            SetClearScore(ScoreManager.Instance.PlayScore);
        }

        resultAnim.SetBool("Show", true);

        AudioManager.Instance?.PlaySE("Sci Fi Beam Reload 1");
        AudioManager.Instance?.PlayBGMWithFade("Three_Keys_(Freestyle_Rap_Beat_No.02)", volume: 0.9f);

        StartCoroutine(PlayFinishAnim());
    }

    /// <summary>
    /// ランクの画像を設定する
    /// </summary>
    /// <param name="playRank"></param>
    private void SetRankSprite(PlayRank playRank)
    {
        switch(playRank)
        {
            case PlayRank.C:
                rankImage.sprite = rankSpriteC;
                break;

            case PlayRank.B:
                rankImage.sprite = rankSpriteB;
                break;

            case PlayRank.A:
                rankImage.sprite = rankSpriteA;
                break;

            case PlayRank.S:
                rankImage.sprite = rankSPriteS;
                break;
        }
    }

    /// <summary>
    /// クリア時間テキストをTimeSpanから設定する
    /// </summary>
    /// <param name="timeSpan"></param>
    private void SetClearTime(TimeSpan timeSpan)
    {
        clearTimeText.text = timeSpan.ToString(@"hh\:mm\:ss");
    }

    /// <summary>
    /// ダメージを受けた回数のテキストを設定する
    /// </summary>
    /// <param name="damegeCount"></param>
    private void SetDamageCount(int damegeCount)
    {
        damageCountText.text = damegeCount.ToString();
    }

    /// <summary>
    /// クリア時のスコアテキストを設定する
    /// </summary>
    /// <param name="score"></param>
    private void SetClearScore(int score)
    {
        clearScoreText.text = score.ToString();
    }

    private IEnumerator PlayFinishAnim()
    {
        yield return new WaitForSeconds(8.0f);

        resultAnim.SetBool("Show", false);

        yield return new WaitForSeconds(0.5f);

        finishAnim.SetTrigger("Show");

        AudioManager.Instance.StopCurrentBGMWithFade(6.0f);

        yield return new WaitForSeconds(6.0f);

        ScoreManager.Instance.ResetScore();

        SceneTransitionManager.Instance.TransitionByName("Title", TransitionType.FadeInOut);
    }
}
