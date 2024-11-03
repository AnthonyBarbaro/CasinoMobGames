using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace war
{
    public class DeckScript : MonoBehaviour
    {
        public Sprite[] cardSprites;
        int[] cardValues = new int[52];
        int currentIndex = 0;

        void Start()
        {
            GetCardValues();
        }

        void GetCardValues()
        {
            // Assign values to the cards
            for (int i = 0; i < cardSprites.Length; i++)
            {
                int value = (i % 13) + 2; // Cards from 2 to 14 (Ace is high)
                cardValues[i] = value;
            }
        }

        public void Shuffle()
        {
            // Standard Fisher-Yates shuffle algorithm
            for (int i = cardSprites.Length - 1; i > 0; i--)
            {
                int j = UnityEngine.Random.Range(0, i + 1);


                // Swap sprites
                Sprite tempSprite = cardSprites[i];
                cardSprites[i] = cardSprites[j];
                cardSprites[j] = tempSprite;

                // Swap values
                int tempValue = cardValues[i];
                cardValues[i] = cardValues[j];
                cardValues[j] = tempValue;
            }
            currentIndex = 0;
        }

        public int DealCard(CardScript cardScript)
        {
            if (currentIndex >= cardSprites.Length)
            {
                // Reshuffle if all cards have been dealt
                Shuffle();
            }

            cardScript.SetSprite(cardSprites[currentIndex]);
            cardScript.SetValue(cardValues[currentIndex]);
            currentIndex++;
            return cardScript.GetValueOfCard();
        }

        public Sprite GetCardBack()
        {
            // Assuming the first sprite is the card back
            return cardSprites[0];
        }
    }
}
