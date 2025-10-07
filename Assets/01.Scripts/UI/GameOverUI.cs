using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class GameOverUI : MonoBehaviour
{
    [Header("Animation Settings")]
    [SerializeField] private float fadeDuration = 1f;     // 알파 페이드 시간
    [SerializeField] private float slideDuration = 1f;    // 이동 시간
    [SerializeField] private Vector3 startOffset = new Vector3(0, 2000, 0); // 시작 오프셋 (위쪽에서 시작)

    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        rectTransform = GetComponent<RectTransform>();

        // 초기 상태
        canvasGroup.alpha = 0f;
        rectTransform.anchoredPosition = startOffset; // 시작 위치 위로 올리기
        gameObject.SetActive(false);
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

        // 초기화
        canvasGroup.alpha = 0f;
        rectTransform.anchoredPosition = startOffset;

        // 동시에 페이드 인 + 슬라이드 다운
        Sequence seq = DOTween.Sequence();
        seq.Append(canvasGroup.DOFade(1f, fadeDuration));
        seq.Join(rectTransform.DOAnchorPosY(rectTransform.anchoredPosition.y - startOffset.y, slideDuration).SetEase(Ease.OutCubic));

        seq.OnComplete(() =>
        {
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        });
    }

    public void HideGameOver()
    {
        Sequence seq = DOTween.Sequence();
        seq.Append(canvasGroup.DOFade(0f, fadeDuration * 0.5f));
        seq.Join(rectTransform.DOAnchorPosY(rectTransform.anchoredPosition.y + startOffset.y, slideDuration * 0.5f).SetEase(Ease.InCubic));

        seq.OnComplete(() => gameObject.SetActive(false));
    }
}
