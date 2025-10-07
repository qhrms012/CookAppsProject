using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class GameOverUI : MonoBehaviour
{
    [Header("Animation Settings")]
    [SerializeField] private float fadeDuration = 1f;     // ���� ���̵� �ð�
    [SerializeField] private float slideDuration = 1f;    // �̵� �ð�
    [SerializeField] private Vector3 startOffset = new Vector3(0, 2000, 0); // ���� ������ (���ʿ��� ����)

    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        rectTransform = GetComponent<RectTransform>();

        // �ʱ� ����
        canvasGroup.alpha = 0f;
        rectTransform.anchoredPosition = startOffset; // ���� ��ġ ���� �ø���
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

        // �ʱ�ȭ
        canvasGroup.alpha = 0f;
        rectTransform.anchoredPosition = startOffset;

        // ���ÿ� ���̵� �� + �����̵� �ٿ�
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
