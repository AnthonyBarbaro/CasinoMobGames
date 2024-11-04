using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace blackjack
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
        public Button hitBtn;
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

        // UI Texts
        public UnityEngine.UI.Text scoreText;
        public UnityEngine.UI.Text dealerScoreText;
        public UnityEngine.UI.Text betsText;
        public UnityEngine.UI.Text cashText;
        public UnityEngine.UI.Text mainText;

        // Card hiding dealer's 2nd card
        public GameObject hideCard;

        // Current bet amount
        private int currentBet = 0;

        // Flag to check if a round is in progress
        private bool roundInProgress = false;

        void Start()
        {
            InitializeGame();
        }

        private void InitializeGame()
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
            dealBtn.gameObject.SetActive(true);
            standBtn.gameObject.SetActive(false);
            hitBtn.gameObject.SetActive(false);
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
            dealerScoreText.gameObject.SetActive(false);
            scoreText.text = "Hand: 0";

            // Add on click listeners to the buttons
            dealBtn.onClick.AddListener(() => DealClicked());
            hitBtn.onClick.AddListener(() => HitClicked());
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
            PlayButtonSound();
            if (currentBet == 0)
            {
                mainText.text = "Please place a bet before dealing.";
                mainText.gameObject.SetActive(true);
                return;
            }

            StartCoroutine(DealInitialCards());
        }


        private IEnumerator DealInitialCards()
        {
            roundInProgress = true;

            // Deactivate bet buttons during the round
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

            GameObject.Find("Deck").GetComponent<DeckScript>().Shuffle();

            // Hide dealer's second card
            hideCard.GetComponent<Renderer>().enabled = true;

            // Adjust buttons visibility
            dealBtn.gameObject.SetActive(false);
            hitBtn.gameObject.SetActive(true);
            standBtn.gameObject.SetActive(true);

            // Deal initial cards with delay
            // Player Card 1
            PlayCardDealSound();
            playerScript.GetCard();
            scoreText.text = "Hand: " + playerScript.handValue.ToString();
            yield return new WaitForSeconds(0.5f);

            // Dealer Card 1
            PlayCardDealSound();
            dealerScript.GetCard();
            yield return new WaitForSeconds(0.5f);

            // Player Card 2
            PlayCardDealSound();
            playerScript.GetCard();
            scoreText.text = "Hand: " + playerScript.handValue.ToString();
            yield return new WaitForSeconds(0.5f);

            // Dealer Card 2 (hidden)
            PlayCardDealSound();
            dealerScript.GetCard();

            // Check for immediate blackjack
            bool player21 = playerScript.handValue == 21 && playerScript.cardIndex == 2;
            bool dealer21 = dealerScript.handValue == 21 && dealerScript.cardIndex == 2;

            if (player21 || dealer21)
            {
                // Reveal dealer's hidden card
                hideCard.GetComponent<Renderer>().enabled = false;
                dealerScoreText.gameObject.SetActive(true);
                dealerScoreText.text = "Hand: " + dealerScript.handValue.ToString();
                yield return new WaitForSeconds(0.5f);
                RoundOver();
            }
        }

        private void HitClicked()
        {
            PlayButtonSound();
            StartCoroutine(PlayerHit());
        }

        private IEnumerator PlayerHit()
        {
            // Check that there is still room on the table
            if (playerScript.cardIndex <= 10)
            {
                PlayCardDealSound();
                playerScript.GetCard();
                scoreText.text = "Hand: " + playerScript.handValue.ToString();
                yield return new WaitForSeconds(0.5f);

                if (playerScript.handValue > 21)
                {
                    RoundOver();
                }
            }
        }

        private void StandClicked()
        {
            PlayButtonSound();
            hitBtn.gameObject.SetActive(false);
            standBtn.gameObject.SetActive(false);
            StartCoroutine(DealerTurn());
        }

        private IEnumerator DealerTurn()
        {
            // Reveal dealer's hidden card
            hideCard.GetComponent<Renderer>().enabled = false;
            dealerScoreText.gameObject.SetActive(true);
            dealerScoreText.text = "Hand: " + dealerScript.handValue.ToString();
            PlayCardDealSound();
            yield return new WaitForSeconds(0.7f);

            while (dealerScript.handValue < 17 && dealerScript.cardIndex < 10)
            {
                PlayCardDealSound();
                dealerScript.GetCard();
                dealerScoreText.text = "Hand: " + dealerScript.handValue.ToString();
                yield return new WaitForSeconds(0.5f);

            }

            RoundOver();
        }

        // Check for winner and loser, hand is over
        void RoundOver()
        {
            roundInProgress = false;

            // Booleans for bust and blackjack
            bool playerBust = playerScript.handValue > 21;
            bool dealerBust = dealerScript.handValue > 21;
            bool player21 = playerScript.handValue == 21 && playerScript.cardIndex == 2;
            bool dealer21 = dealerScript.handValue == 21 && dealerScript.cardIndex == 2;

            // All bust, bets returned
            if (playerBust && dealerBust)
            {
                mainText.text = "All Bust: Bets returned";
                playerScript.AdjustMoney(currentBet);
            }
            else if (player21 && !dealer21)
            {
                mainText.text = "Blackjack! You win!";
                PlayMoneySound();
                playerScript.AdjustMoney((int)(currentBet * 2.5f)); // Payout 3:2
            }
            else if (dealer21 && !player21)
            {
                mainText.text = "Dealer blackjack!";
                // Player loses bet
            }
            else if (playerBust)
            {
                mainText.text = "Bust! Dealer wins!";
                // Player loses bet
            }
            else if (dealerBust)
            {
                mainText.text = "Dealer busts! You win!";
                playerScript.AdjustMoney(currentBet * 2);
            }
            else if (playerScript.handValue > dealerScript.handValue)
            {
                mainText.text = "You win!";
                PlayMoneySound();
                playerScript.AdjustMoney(currentBet * 2);
            }
            else if (playerScript.handValue == dealerScript.handValue)
            {
                mainText.text = "Push: Bets returned";
                PlayMoneySound();
                playerScript.AdjustMoney(currentBet);
            }
            else
            {
                mainText.text = "Dealer wins!";
                // Player loses bet
            }

            // Set UI up for next move / hand / turn
            hitBtn.gameObject.SetActive(false);
            standBtn.gameObject.SetActive(false);
            dealBtn.gameObject.SetActive(true);
            mainText.gameObject.SetActive(true);
            dealerScoreText.gameObject.SetActive(true);
            hideCard.GetComponent<Renderer>().enabled = false;
            cashText.text = "$" + playerScript.GetMoney().ToString();

            // Reactivate bet buttons for next round
            betBtn5.gameObject.SetActive(true);
            betBtn20.gameObject.SetActive(true);
            betBtn100.gameObject.SetActive(true);
            betBtn500.gameObject.SetActive(true);

            // Reset current bet
            currentBet = 0;
            betsText.text = "Bet: $0";

            if (playerScript.GetMoney() <= 0)
            {
                GameOver();
            }
        }

        private void GameOver()
        {
            mainText.gameObject.SetActive(true);
            dealBtn.gameObject.SetActive(false);
            hitBtn.gameObject.SetActive(false);
            standBtn.gameObject.SetActive(false);
            mainText.text = "Game Over!";
            // Show restart game button
            restartGameBtn.gameObject.SetActive(true);
        }

        private void RestartGame()
        {
            playerScript.ResetMoney();
            cashText.text = "$" + playerScript.GetMoney().ToString();
            restartGameBtn.gameObject.SetActive(false);
            mainText.gameObject.SetActive(false);

            // Reactivate bet buttons
            betBtn5.gameObject.SetActive(true);
            betBtn20.gameObject.SetActive(true);
            betBtn100.gameObject.SetActive(true);
            betBtn500.gameObject.SetActive(true);

            // Reset bets text
            betsText.text = "Bet: $0";
            currentBet = 0;

            // Reset dealer and player hands
            playerScript.ResetHand();
            dealerScript.ResetHand();

            // Reset UI texts
            scoreText.text = "Hand: 0";
            dealerScoreText.text = "Hand: 0";
            dealerScoreText.gameObject.SetActive(false);

            // Make deal button visible again
            dealBtn.gameObject.SetActive(true);
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
        // Set bet amount when bet button is clicked
        void BetClicked(int value)
        {

            if (roundInProgress)
            {
                // Can't change bet during a round
                mainText.text = "Cannot change bet during a round.";
                mainText.gameObject.SetActive(true);
                return;
            }

            // Check if player has enough money to add to the bet
            if (playerScript.GetMoney() >= value)
            {
                PlayMoneySound();
                currentBet += value;
                playerScript.AdjustMoney(-value);
                betsText.text = "Bet: $" + currentBet.ToString();
                cashText.text = "$" + playerScript.GetMoney().ToString();
                clearBetBtn.gameObject.SetActive(true);
            }
            else
            {
                mainText.text = "Not enough money to increase the bet.";
                mainText.gameObject.SetActive(true);
            }
        }
    }
}
