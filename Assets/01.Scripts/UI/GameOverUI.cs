using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class GameOverUI : MonoBehaviour
{
    [Header("Animation Settings")]
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private float slideDuration = 1f;
    [SerializeField] private Vector3 startOffset = new Vector3(0, 2000f, 0);

    [Header("UI References")]
    [SerializeField] private Image panel;
    [SerializeField] private TextMeshProUGUI gameOverText;

    private RectTransform rectTransform;
    private Color panelColor;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        panelColor = panel.color;

        // 초기 상태
        panelColor.a = 0f;
        panel.color = panelColor;
        rectTransform.anchoredPosition = startOffset;
    }

    private void OnEnable()
    {
        GameManager.onGameOver += ShowGameOver;
    }

    private void OnDisable()
    {
        GameManager.onGameOver -= ShowGameOver;
    }

    public void ShowGameOver(bool isWin)
    {
        gameObject.SetActive(true);
        gameOverText.text = isWin ? "성공" : "실패";

        AudioManager.Instance.PlaySfx(isWin ? AudioManager.Sfx.GameClear : AudioManager.Sfx.GameOver);

        panelColor.a = 0f;
        panel.color = panelColor;
        rectTransform.anchoredPosition = startOffset;

        Sequence seq = DOTween.Sequence();
        seq.Append(panel.DOFade(1f, fadeDuration));
        seq.Join(rectTransform.DOAnchorPosY(rectTransform.anchoredPosition.y - startOffset.y, slideDuration)
            .SetEase(Ease.OutCubic));

    }

    public void HideGameOver()
    {
        Sequence seq = DOTween.Sequence();
        seq.Append(panel.DOFade(0f, fadeDuration * 0.5f));
        seq.Join(rectTransform.DOAnchorPosY(rectTransform.anchoredPosition.y + startOffset.y, slideDuration * 0.5f)
            .SetEase(Ease.InCubic));
        seq.OnComplete(() => gameObject.SetActive(false));
    }
}

