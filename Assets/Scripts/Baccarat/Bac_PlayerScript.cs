﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace baccarat
{
    public class PlayerScript : MonoBehaviour
    {
        public DeckScript deckScript;

        public int handValue = 0;
        private int money = 1000;

        public GameObject[] hand;
        public int cardIndex = 0;
        List<CardScript> aceList = new List<CardScript>();

        public void StartHand()
        {
            GetCard();
            GetCard();
        }

        public int GetCard()
        {
            int cardValue = deckScript.DealCard(hand[cardIndex].GetComponent<CardScript>());
            hand[cardIndex].GetComponent<Renderer>().enabled = true;
            handValue += cardValue;
            cardIndex++;
            return handValue;
        }

        public void AdjustMoney(int amount)
        {
            money += amount;
            if (money < 0)
            {
                money = 0;
            }
        }

        public int GetMoney()
        {
            return money;
        }

        public void ResetMoney(int amount)
        {
            money = amount;
        }

        public void ResetHand()
        {
            for (int i = 0; i < hand.Length; i++)
            {
                hand[i].GetComponent<CardScript>().ResetCard();
                hand[i].GetComponent<Renderer>().enabled = false;
            }
            cardIndex = 0;
            handValue = 0;
            aceList = new List<CardScript>();
        }
    }
}
