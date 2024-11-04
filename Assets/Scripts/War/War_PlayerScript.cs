using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace war
{
    public class PlayerScript : MonoBehaviour
    {
        // This script is for BOTH player and dealer

        // Get other scripts
        public DeckScript deckScript;

        // Total value of player/dealer's hand
        public int handValue = 0;

        // Betting money
        private int money = 1000;

        // Array of card objects on table
        public GameObject[] hand;
        // Index of next card to be turned over
        public int cardIndex = 0;

        public void StartHand()
        {
            GetCard();
        }

        // Add a card to the player/dealer's hand
        public int GetCard()
        {
            // Get a card, use deal card to assign sprite and value to card on table
            int cardValue = deckScript.DealCard(hand[cardIndex].GetComponent<CardScript>());
            // Show card on game screen
            hand[cardIndex].GetComponent<Renderer>().enabled = true;
            // Set handValue to the card's value
            handValue = cardValue;
            cardIndex++;
            return handValue;
        }

        // Add or subtract from the global cash, for bets
        public void AdjustMoney(int amount)
        {
            GlobalGameManager.Instance.AdjustPlayerCash(amount);
        }

        // Output player's current money amount from the global cash system
        public int GetMoney()
        {
            return GlobalGameManager.Instance.GetPlayerCash();
        }

        // Hides all cards, resets the needed variables
        public void ResetHand()
        {
            for (int i = 0; i < hand.Length; i++)
            {
                hand[i].GetComponent<CardScript>().ResetCard();
                hand[i].GetComponent<Renderer>().enabled = false;
            }
            cardIndex = 0;
            handValue = 0;
        }
    }
}
