﻿using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace baccarat {
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
            int num = 0;
            // Loop to assign values to the cards
            for (int i = 0; i < cardSprites.Length; i++)
            {
                num = i + 2;
                // Count up to the amout of cards, 52
                num %= 13;
                // if there is a remainder after x/13, then remainder
                // is used as the value, unless over 10, the use 10
                if(num > 9)
                {
                    num = 0;
                }
                cardValues[i] = num;
            }
        }

        public void Shuffle()
        {
            // Standard array data swapping technique
            for(int i = cardSprites.Length -1; i > 0; --i)
            {
                int j = Mathf.FloorToInt(Random.Range(0.0f, 1.0f) * cardSprites.Length - 1) + 1;
                Sprite face = cardSprites[i];
                cardSprites[i] = cardSprites[j];
                cardSprites[j] = face;

                int value = cardValues[i];
                cardValues[i] = cardValues[j];
                cardValues[j] = value;
            }
            currentIndex = 1;
        }

        public int DealCard(CardScript cardScript)
        {
            cardScript.SetSprite(cardSprites[currentIndex]);
            cardScript.SetValue(cardValues[currentIndex]);
            currentIndex++;
            return cardScript.GetValueOfCard();
        }

        public Sprite GetCardBack()
        {
            return cardSprites[0];
        }
    }
}