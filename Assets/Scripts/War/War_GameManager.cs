using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using System.Diagnostics;

namespace war
{
    public class GameManager : MonoBehaviour
    {
        //Audio 
        private AudioSource audioSource;
        public AudioClip moneyClip;
        public AudioClip cardDealClip;
        public AudioClip buttonClip;
        // Game Buttons
        public Button dealBtn;
        public Button standBtn;
        public Button betBtn5;
        public Button betBtn20;
        public Button betBtn100;
        public Button betBtn500;
        public Button backBtn;
        public Button restartGameBtn;
        public Button clearBetBtn;

        // Access the player and dealer's script
        public PlayerScript playerScript;
        public PlayerScript dealerScript;

        // UI Text elements
        public UnityEngine.UI.Text scoreText;
        public UnityEngine.UI.Text dealerScoreText;
        public UnityEngine.UI.Text betsText;
        public UnityEngine.UI.Text cashText;
        public UnityEngine.UI.Text mainText;

        // Cards hiding player's and dealer's cards
        public GameObject hideCard;
        public GameObject hidePlayerCard;

        // Current bet amount
        private int currentBet = 0;
        private bool roundInProgress = false;

        void Start()
        {
            // Ensure the GameObject has an AudioSource component
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }

            if (moneyClip == null)
                UnityEngine.Debug.LogWarning("Money clip is not assigned.");
            if (cardDealClip == null)
                UnityEngine.Debug.LogWarning("Card deal clip is not assigned.");
            if (buttonClip == null)
                UnityEngine.Debug.LogWarning("Card deal clip is not assigned.");
            // Initialize buttons
            standBtn.gameObject.SetActive(false);
            betBtn5.gameObject.SetActive(true);
            betBtn20.gameObject.SetActive(true);
            betBtn100.gameObject.SetActive(true);
            betBtn500.gameObject.SetActive(true);
            restartGameBtn.gameObject.SetActive(false);
            clearBetBtn.gameObject.SetActive(false);

            // Initialize UI texts
            betsText.text = "Bet: $0";
            cashText.text = "$" + playerScript.GetMoney().ToString();
            mainText.gameObject.SetActive(false);
            scoreText.text = "Hand: ?";
            dealerScoreText.text = "Hand: ?";

            // Add on click listeners to the buttons
            dealBtn.onClick.AddListener(() => DealClicked());
            standBtn.onClick.AddListener(() => StandClicked());
            betBtn5.onClick.AddListener(() => BetClicked(5));
            betBtn20.onClick.AddListener(() => BetClicked(20));
            betBtn100.onClick.AddListener(() => BetClicked(100));
            betBtn500.onClick.AddListener(() => BetClicked(500));
            backBtn.onClick.AddListener(() => Back());
            restartGameBtn.onClick.AddListener(() => RestartGame());
            clearBetBtn.onClick.AddListener(() => ClearBet());
        }
        private void PlayMoneySound()
        {
            if (moneyClip != null)
            {
                audioSource.PlayOneShot(moneyClip);
            }
        }
        private void PlayButtonSound()
        {
            if (buttonClip != null)
            {
                audioSource.PlayOneShot(buttonClip);
            }
        }

        private void PlayCardDealSound()
        {
            if (cardDealClip != null)
            {
                audioSource.PlayOneShot(cardDealClip);
            }
        }
        private void Back()
        {
            PlayButtonSound();
            SceneManager.LoadScene("MenuScene");
        }

        private void DealClicked()
        {
            if (currentBet == 0)
            {
                mainText.text = "Please place a bet before dealing.";
                mainText.gameObject.SetActive(true);
                return;
            }
            PlayButtonSound();
            StartCoroutine(DealCards());
        }

        private IEnumerator DealCards()
        {
            roundInProgress = true;

            // Disable buttons during the round
            dealBtn.gameObject.SetActive(false);
            betBtn5.gameObject.SetActive(false);
            betBtn20.gameObject.SetActive(false);
            betBtn100.gameObject.SetActive(false);
            betBtn500.gameObject.SetActive(false);
            clearBetBtn.gameObject.SetActive(false);

            // Reset round, hide text, prep for new hand
            playerScript.ResetHand();
            dealerScript.ResetHand();
            dealerScoreText.gameObject.SetActive(false);
            mainText.gameObject.SetActive(false);

            // Hide cards
            hideCard.GetComponent<Renderer>().enabled = true;
            hidePlayerCard.GetComponent<Renderer>().enabled = true;

            // Shuffle deck
            GameObject.Find("Deck").GetComponent<DeckScript>().Shuffle();

            // Deal cards with delay
            playerScript.StartHand();
            PlayCardDealSound();
            yield return new WaitForSeconds(0.5f);

            dealerScript.StartHand();
            PlayCardDealSound();
            yield return new WaitForSeconds(0.5f);

            // Update the scores displayed
            scoreText.text = "Hand: ?";
            dealerScoreText.text = "Hand: ?";

            standBtn.gameObject.SetActive(true);
        }

        private string CalHand(int value)
        {
            if (value <= 10)
            {
                return (value).ToString();
            }
            else if (value == 11)
            {
                return "J";
            }
            else if (value == 12)
            {
                return "Q";
            }
            else if (value == 13)
            {
                return "K";
            }
            else // value == 14
            {
                return "A";
            }
        }

        private void StandClicked()
        {
            if (!roundInProgress)
                return;
            PlayButtonSound();
            StartCoroutine(RoundOver());
        }

        private IEnumerator RoundOver()
        {
            roundInProgress = false;

            // Reveal cards with delay
            hideCard.GetComponent<Renderer>().enabled = false;
            hidePlayerCard.GetComponent<Renderer>().enabled = false;
            yield return new WaitForSeconds(0.5f);

            scoreText.text = "Hand: " + CalHand(playerScript.handValue);
            dealerScoreText.text = "Hand: " + CalHand(dealerScript.handValue);
            dealerScoreText.gameObject.SetActive(true);

            if (playerScript.handValue > dealerScript.handValue)
            {
                mainText.text = "You Win!";
                playerScript.AdjustMoney(currentBet * 2); // Payout is 1:1 (original bet + winnings)
                PlayMoneySound();
            }
            else if (playerScript.handValue < dealerScript.handValue)
            {
                mainText.text = "Dealer Wins!";
                // Player loses bet (already deducted when placing bet)
            }
            else
            {
                mainText.text = "Push: Bet returned";
                playerScript.AdjustMoney(currentBet); // Return original bet
            }

            mainText.gameObject.SetActive(true);
            cashText.text = "$" + playerScript.GetMoney().ToString();

            // Reactivate buttons for next round
            dealBtn.gameObject.SetActive(true);
            betBtn5.gameObject.SetActive(true);
            betBtn20.gameObject.SetActive(true);
            betBtn100.gameObject.SetActive(true);
            betBtn500.gameObject.SetActive(true);
            clearBetBtn.gameObject.SetActive(false);
            standBtn.gameObject.SetActive(false);

            // Reset current bet
            currentBet = 0;
            betsText.text = "Bet: $0";

            if (playerScript.GetMoney() <= 0)
            {
                GameOver();
            }

            yield return null;
        }

        private void GameOver()
        {
            mainText.gameObject.SetActive(true);
            mainText.text = "Game Over!";
            restartGameBtn.gameObject.SetActive(true);
            dealBtn.gameObject.SetActive(false);
            standBtn.gameObject.SetActive(false);
            betBtn5.gameObject.SetActive(false);
            betBtn20.gameObject.SetActive(false);
            betBtn100.gameObject.SetActive(false);
            betBtn500.gameObject.SetActive(false);
            clearBetBtn.gameObject.SetActive(false);
        }

        private void RestartGame()
        {
            PlayButtonSound();
            playerScript.ResetHand();
            dealerScript.ResetHand();
            playerScript.AdjustMoney(1000 - playerScript.GetMoney()); // Reset money to 1000
            cashText.text = "$" + playerScript.GetMoney().ToString();
            restartGameBtn.gameObject.SetActive(false);
            mainText.gameObject.SetActive(false);

            // Reactivate buttons
            dealBtn.gameObject.SetActive(true);
            betBtn5.gameObject.SetActive(true);
            betBtn20.gameObject.SetActive(true);
            betBtn100.gameObject.SetActive(true);
            betBtn500.gameObject.SetActive(true);

            // Reset UI elements
            betsText.text = "Bet: $0";
            scoreText.text = "Hand: ?";
            dealerScoreText.text = "Hand: ?";
            dealerScoreText.gameObject.SetActive(false);
            hideCard.GetComponent<Renderer>().enabled = false;
            hidePlayerCard.GetComponent<Renderer>().enabled = false;
        }

        // Add money to bet if bet clicked
        void BetClicked(int value)
        {
            if (roundInProgress)
            {
                mainText.text = "Cannot change bet during a round.";
                mainText.gameObject.SetActive(true);
                return;
            }

            if (playerScript.GetMoney() >= value)
            {
                PlayMoneySound();
                currentBet += value;
                playerScript.AdjustMoney(-value);
                betsText.text = "Bet: $" + currentBet.ToString();
                cashText.text = "$" + playerScript.GetMoney().ToString();
                clearBetBtn.gameObject.SetActive(true);
                mainText.gameObject.SetActive(false); // Hide any previous messages
            }
            else
            {
                mainText.text = "Not enough cash to place this bet.";
                mainText.gameObject.SetActive(true);
            }
        }

        void ClearBet()
        {
            if (roundInProgress)
            {
                mainText.text = "Cannot clear bet during a round.";
                mainText.gameObject.SetActive(true);
                return;
            }
            PlayMoneySound();
            playerScript.AdjustMoney(currentBet);
            currentBet = 0;
            betsText.text = "Bet: $0";
            cashText.text = "$" + playerScript.GetMoney().ToString();
            clearBetBtn.gameObject.SetActive(false);
        }
    }
}
