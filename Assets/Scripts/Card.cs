using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Card : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer cardRenderer;

    [SerializeField]
    private Sprite animalSprite;

    [SerializeField]
    private Sprite backSprite;

    private bool isFlipped = false;
    private bool isFilpping = false;
    private bool isMatched = false;

    public int cardID;

    public void SetCardID(int id) {
        cardID = id;
    }

    public void SetMatched() {
        isMatched = true;
    }

    public void SetAnimalSprite(Sprite sprite) {
        animalSprite = sprite;
    }

    public void FlipCard() {
        isFilpping = true;

        Vector3 originalScale = transform.localScale;
        Vector3 targetScale = new Vector3(0f, originalScale.y, originalScale.z);

        // DOScale(a, b) : a스케일 값을 b만큼의 시간동안 변경
        transform.DOScale(targetScale, 0.2f).OnComplete(() =>
        {
            isFlipped = !isFlipped;

            if (isFlipped) {
                cardRenderer.sprite = animalSprite;
            } else {
                cardRenderer.sprite = backSprite;
            }

            transform.DOScale(originalScale, 0.2f).OnComplete(() =>{
                isFilpping = false;
            });
        });
    }

    void OnMouseDown() {
        if(!isFilpping && !isMatched && !isFlipped) {
            GameManager.instance.CardClicked(this);
        }
    }

}
