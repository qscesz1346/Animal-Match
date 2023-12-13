using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    private List<Card> allCards;
    private Card flippedCard;
    private bool isFlipping = false;

    [SerializeField]
    private Slider timeoutSlider;
    [SerializeField]
    private TextMeshProUGUI timeoutText;
    [SerializeField]
    private TextMeshProUGUI gameOverText;
    [SerializeField]
    private GameObject gameOverPanel;
    private bool isGameOver = false;
    [SerializeField]
    private TextMeshProUGUI usageTimeText;
    [SerializeField]
    private float timeLimit = 60f;
    private float currentTime;
    private float usageTime;
    private int totalMatches = 10;
    private int matchesFound = 0;

    void Awake() {
        if(instance == null) {
            instance = this;
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        Board board = FindObjectOfType<Board>();
        allCards = board.GetCrads();

        currentTime = timeLimit;
        SetCurrentTimeText();
        StartCoroutine("FlipAllCardsRoutine");
    }

    void SetCurrentTimeText() {
        int timeSec = Mathf.CeilToInt(currentTime);
        timeoutText.SetText(timeSec.ToString());
    }

    IEnumerator FlipAllCardsRoutine() {
        isFlipping = true;
        yield return new WaitForSeconds(0.5f);
        FlipAllCards();
        yield return new WaitForSeconds(3f);
        FlipAllCards();
        yield return new WaitForSeconds(0.5f);
        isFlipping = false;

        yield return StartCoroutine("CountDownTimerRoutine");
    }

    IEnumerator CountDownTimerRoutine() {
        while (currentTime > 0) {
            currentTime -= Time.deltaTime;
            timeoutSlider.value = currentTime / timeLimit;
            SetCurrentTimeText();
            usageTime = timeLimit - currentTime;
            yield return null;
        }

        GameOver(false);
    }

    void FlipAllCards() {
        foreach (Card card in allCards)  {
            card.FlipCard();
        }
    }

    public void CardClicked(Card card) {
        if (isFlipping || isGameOver) {
            return;
        }
        
        card.FlipCard();

        if (flippedCard == null) {
            flippedCard = card;
        } else {
            // check match
            StartCoroutine(CheckMatchRoutine(flippedCard, card));
        }
    }

    IEnumerator CheckMatchRoutine(Card card1, Card card2) {
        isFlipping = true;
        
        if (card1.cardID == card2.cardID) {
            card1.SetMatched();
            card2.SetMatched();

            matchesFound++;
            if(matchesFound == totalMatches) {
                GameOver(true);
            }
        } else {
            
            yield return new WaitForSeconds(1f);

            card1.FlipCard();
            card2.FlipCard();

            yield return new WaitForSeconds(0.4f);
        }

        isFlipping = false;
        flippedCard = null;
    }

    void GameOver(bool success) {
        
        if(!isGameOver) {

            isGameOver = true;
            StopCoroutine("CountDownTimerRoutine");
            
            if(success) {
                gameOverText.SetText("Good Job");
            } else {
                gameOverText.SetText("Game Over");
            }

            usageTimeText.SetText("Time : " + usageTime.ToString("#.##"));

            Invoke("ShowGameOverPanel", 2f);

        }
    }

    void ShowGameOverPanel() {
        gameOverPanel.SetActive(true);
    }

    public void Restart() {
        SceneManager.LoadScene("SampleScene");
    }
}
