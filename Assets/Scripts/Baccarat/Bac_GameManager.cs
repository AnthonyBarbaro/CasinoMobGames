using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Ensure this is included
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using System.Diagnostics;

namespace baccarat
{
    public class GameManager : MonoBehaviour
    {
        //Audio 
        private AudioSource audioSource;
        public AudioClip moneyClip;
        public AudioClip cardDealClip;
        // Game Buttons
        public Button dealBtn;
        public Button betBtn5;
        public Button betBtn20;
        public Button betBtn100;
        public Button betBtn500;
        public Button tieBetBtn;
        public Button bankBetBtn;
        public Button playerBetBtn;
        public Button tieBetTxtBtn;
        public Button bankBetTxtBtn;
        public Button playerBetTxtBtn;
        public Button tieBetTitleBtn;
        public Button bankBetTitleBtn;
        public Button playerBetTitleBtn;
        public Button newGameBtn;
        public Button restartGameBtn;
        public Button backBtn;
        public Button clearBetBtn;

        public GameObject hideCard1;
        public GameObject hideCard2;
        public GameObject hideCard3;
        public GameObject hideCard4;

        private int potBank = 0;
        private int potTie = 0;
        private int potPlayer = 0;
        private int betType = 0; // 1: Tie, 2: Banker, 3: Player
        private bool roundInProgress = false;

        // Access the player and dealer's script
        public PlayerScript playerScript;
        public PlayerScript dealerScript;

        // UI Text elements (Fully qualify to avoid ambiguity)
        public UnityEngine.UI.Text scoreText;
        public UnityEngine.UI.Text dealerScoreText;
        public UnityEngine.UI.Text betsTieText;
        public UnityEngine.UI.Text betsBankText;
        public UnityEngine.UI.Text betsPlayerText;
        public UnityEngine.UI.Text cashText;
        public UnityEngine.UI.Text mainText;

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
            // Initialize button visibility and event listeners
            dealBtn.gameObject.SetActive(false);
            betBtn5.gameObject.SetActive(false);
            betBtn20.gameObject.SetActive(false);
            betBtn100.gameObject.SetActive(false);
            betBtn500.gameObject.SetActive(false);
            newGameBtn.gameObject.SetActive(true);
            restartGameBtn.gameObject.SetActive(false);
            clearBetBtn.gameObject.SetActive(false);

            DisableBetTypeButtons(false);

            tieBetBtn.onClick.AddListener(() => SetBetType(1));
            bankBetBtn.onClick.AddListener(() => SetBetType(2));
            playerBetBtn.onClick.AddListener(() => SetBetType(3));
            tieBetTxtBtn.onClick.AddListener(() => SetBetType(1));
            bankBetTxtBtn.onClick.AddListener(() => SetBetType(2));
            playerBetTxtBtn.onClick.AddListener(() => SetBetType(3));
            tieBetTitleBtn.onClick.AddListener(() => SetBetType(1));
            bankBetTitleBtn.onClick.AddListener(() => SetBetType(2));
            playerBetTitleBtn.onClick.AddListener(() => SetBetType(3));

            dealBtn.onClick.AddListener(() => StartCoroutine(DealClicked()));
            newGameBtn.onClick.AddListener(() => Reload());
            restartGameBtn.onClick.AddListener(() => RestartGame());
            betBtn5.onClick.AddListener(() => BetClicked(5));
            betBtn20.onClick.AddListener(() => BetClicked(20));
            betBtn100.onClick.AddListener(() => BetClicked(100));
            betBtn500.onClick.AddListener(() => BetClicked(500));
            backBtn.onClick.AddListener(() => Back());
            clearBetBtn.onClick.AddListener(() => ClearBet());

            // Initialize cash display
            UpdateCashDisplay();
            betsBankText.text = "Bets: $0";
            betsTieText.text = "Bets: $0";
            betsPlayerText.text = "Bets: $0";
        }
        private void PlayMoneySound()
        {
            if (moneyClip != null)
            {
                audioSource.PlayOneShot(moneyClip);
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
            SceneManager.LoadScene("MenuScene");
        }

        private void Reload()
        {
            if (playerScript.GetMoney() == 0)
            {
                GameOver();
            }
            else
            {
                // Reset the game state for a new round
                hideCard1.gameObject.SetActive(true);
                hideCard2.gameObject.SetActive(true);
                hideCard3.gameObject.SetActive(true);
                hideCard4.gameObject.SetActive(true);

                dealBtn.gameObject.SetActive(false);
                betBtn5.gameObject.SetActive(false);
                betBtn20.gameObject.SetActive(false);
                betBtn100.gameObject.SetActive(false);
                betBtn500.gameObject.SetActive(false);
                newGameBtn.gameObject.SetActive(false);
                scoreText.gameObject.SetActive(false);
                dealerScoreText.gameObject.SetActive(false);
                mainText.gameObject.SetActive(false);
                clearBetBtn.gameObject.SetActive(false);

                betType = 0;
                DisableBetTypeButtons(false);
                dealerScript.ResetHand();
                playerScript.ResetHand();
                roundInProgress = false;
            }
        }

        private void SetBetType(int value)
        {
            if (roundInProgress)
            {
                mainText.text = "Cannot change bet during a round.";
                mainText.gameObject.SetActive(true);
                StartCoroutine(HideMessageAfterDelay(1f));
                return;
            }

            betType = value;
            betBtn5.gameObject.SetActive(true);
            betBtn20.gameObject.SetActive(true);
            betBtn100.gameObject.SetActive(true);
            betBtn500.gameObject.SetActive(true);
            clearBetBtn.gameObject.SetActive(true);

            HideOtherBetButtons();
        }

        private void HideOtherBetButtons()
        {
            tieBetBtn.gameObject.SetActive(betType != 1);
            bankBetBtn.gameObject.SetActive(betType != 2);
            playerBetBtn.gameObject.SetActive(betType != 3);
        }

        private void DisableBetTypeButtons(bool disable)
        {
            tieBetBtn.interactable = !disable;
            bankBetBtn.interactable = !disable;
            playerBetBtn.interactable = !disable;
            tieBetTxtBtn.interactable = !disable;
            bankBetTxtBtn.interactable = !disable;
            playerBetTxtBtn.interactable = !disable;
            tieBetTitleBtn.interactable = !disable;
            bankBetTitleBtn.interactable = !disable;
            playerBetTitleBtn.interactable = !disable;
        }

        private IEnumerator DealClicked()
        {
            if (roundInProgress)
            {
                yield break;
            }

            roundInProgress = true;
            PlayCardDealSound();
            // Deactivate betting buttons
            betBtn5.gameObject.SetActive(false);
            betBtn20.gameObject.SetActive(false);
            betBtn100.gameObject.SetActive(false);
            betBtn500.gameObject.SetActive(false);
            clearBetBtn.gameObject.SetActive(false);

            // Deactivate deal button
            dealBtn.gameObject.SetActive(false);

            // Deactivate hide cards
            hideCard1.gameObject.SetActive(false);
            hideCard2.gameObject.SetActive(false);
            hideCard3.gameObject.SetActive(false);
            hideCard4.gameObject.SetActive(false);

            DisableBetTypeButtons(true);
            mainText.gameObject.SetActive(false);

            // Shuffle deck
            GameObject.Find("Deck").GetComponent<DeckScript>().Shuffle();

            // Start dealing cards with delays
            playerScript.StartHand();
            dealerScript.StartHand();

            scoreText.text = "Player's hand: " + (playerScript.handValue % 10).ToString();
            dealerScoreText.text = "Banker's hand: " + (dealerScript.handValue % 10).ToString();
            scoreText.gameObject.SetActive(true);
            dealerScoreText.gameObject.SetActive(true);

            yield return new WaitForSeconds(0.5f);

            // Proceed to evaluate the round
            yield return StartCoroutine(RoundOver());
        }

        private IEnumerator RoundOver()
        {
            int playerValue = playerScript.handValue % 10;
            int dealerValue = dealerScript.handValue % 10;

            yield return new WaitForSeconds(1f);

            // Player's third card
            int playerThirdCardValue = -1;
            if (playerValue <= 5)
            {
                PlayCardDealSound();
                playerScript.GetCard();
                
                playerThirdCardValue = playerScript.hand[playerScript.cardIndex - 1].GetComponent<CardScript>().GetValueOfCard();
                scoreText.text = "Player's hand: " + (playerScript.handValue % 10).ToString();
                yield return new WaitForSeconds(0.5f);
            }

            // Banker's turn
            dealerValue = dealerScript.handValue % 10;

            bool dealerDraws = false;
            if (dealerValue <= 2)
            {
                dealerDraws = true;
            }
            else if (dealerValue == 3 && playerThirdCardValue != 8)
            {
                dealerDraws = true;
            }
            else if (dealerValue == 4 && playerThirdCardValue >= 2 && playerThirdCardValue <= 7)
            {
                dealerDraws = true;
            }
            else if (dealerValue == 5 && playerThirdCardValue >= 4 && playerThirdCardValue <= 7)
            {
                dealerDraws = true;
            }
            else if (dealerValue == 6 && playerThirdCardValue >= 6 && playerThirdCardValue <= 7)
            {
                dealerDraws = true;
            }

            if (dealerDraws)
            {
                PlayCardDealSound();
                dealerScript.GetCard();
                
                dealerScoreText.text = "Banker's hand: " + (dealerScript.handValue % 10).ToString();
                yield return new WaitForSeconds(0.5f);
            }

            int playerFinalValue = playerScript.handValue % 10;
            int bankerFinalValue = dealerScript.handValue % 10;

            yield return new WaitForSeconds(1f);

            // Determine the winner
            if (playerFinalValue > bankerFinalValue)
            {
                mainText.text = "Player Wins!";
                playerScript.AdjustMoney(potPlayer * 2); // Pays 1:1
               
            }
            else if (playerFinalValue < bankerFinalValue)
            {
                mainText.text = "Banker Wins!";
                playerScript.AdjustMoney((int)(potBank * 1.95f)); // Pays 0.95:1 (5% commission)
                
            }
            else
            {
                mainText.text = "Tie!";
                playerScript.AdjustMoney(potTie * 9); // Pays 8:1 (total return is 9x the bet)
                // Return bets on Player and Banker (as per Baccarat rules)
                playerScript.AdjustMoney(potPlayer);
                playerScript.AdjustMoney(potBank);
                
            }

            // Reset pots and bets
            potTie = 0;
            potBank = 0;
            potPlayer = 0;
            betsBankText.text = "Bets: $0";
            betsTieText.text = "Bets: $0";
            betsPlayerText.text = "Bets: $0";
            mainText.gameObject.SetActive(true);
            newGameBtn.gameObject.SetActive(true);
            roundInProgress = false;

            UpdateCashDisplay();
            PlayMoneySound();
            if (playerScript.GetMoney() == 0)
            {
                GameOver();
            }
        }

        private void GameOver()
        {
            mainText.gameObject.SetActive(true);
            mainText.text = "Game Over!";
            DisableBetTypeButtons(true);
            restartGameBtn.gameObject.SetActive(true);
            newGameBtn.gameObject.SetActive(false);
            dealBtn.gameObject.SetActive(false);
            betBtn5.gameObject.SetActive(false);
            betBtn20.gameObject.SetActive(false);
            betBtn100.gameObject.SetActive(false);
            betBtn500.gameObject.SetActive(false);
            clearBetBtn.gameObject.SetActive(false);
        }

        private void RestartGame()
        {
            playerScript.ResetMoney(1000);
            UpdateCashDisplay();
            PlayMoneySound();
            restartGameBtn.gameObject.SetActive(false);
            Reload();
        }

        void BetClicked(int value)
        {
            if (roundInProgress)
            {
                mainText.text = "Cannot change bet during a round.";
                mainText.gameObject.SetActive(true);
                StartCoroutine(HideMessageAfterDelay(1f));
                return;
            }

            if (playerScript.GetMoney() >= value)
            {
                playerScript.AdjustMoney(-value);
                if (betType == 1)
                {
                    potTie += value;
                    betsTieText.text = "Bets: $" + potTie.ToString();
                }
                else if (betType == 2)
                {
                    potBank += value;
                    betsBankText.text = "Bets: $" + potBank.ToString();
                }
                else if (betType == 3)
                {
                    potPlayer += value;
                    betsPlayerText.text = "Bets: $" + potPlayer.ToString();
                }
                UpdateCashDisplay();
                PlayMoneySound();
                dealBtn.gameObject.SetActive(true);
                clearBetBtn.gameObject.SetActive(true);
            }
            else
            {
                mainText.text = "Not enough cash!";
                mainText.gameObject.SetActive(true);
                StartCoroutine(HideMessageAfterDelay(1f));
            }
        }

        private IEnumerator HideMessageAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            mainText.gameObject.SetActive(false);
        }

        private void UpdateCashDisplay()
        {
            cashText.text = "$" + playerScript.GetMoney().ToString();
            
        }

        void ClearBet()
        {
            if (roundInProgress)
            {
                mainText.text = "Cannot clear bet during a round.";
                mainText.gameObject.SetActive(true);
                StartCoroutine(HideMessageAfterDelay(1f));
                return;
            }

            playerScript.AdjustMoney(potBank + potTie + potPlayer);

            potBank = 0;
            potTie = 0;
            potPlayer = 0;
            betsBankText.text = "Bets: $0";
            betsTieText.text = "Bets: $0";
            betsPlayerText.text = "Bets: $0";
            PlayMoneySound();
            UpdateCashDisplay();
            clearBetBtn.gameObject.SetActive(false);
            dealBtn.gameObject.SetActive(false);
        }
    }
}
