using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.scripts
{
    class Player
    {
        public GameObject Panel { get; set; }
        public int NumberOfCards { get; set; }
        public List<Card> Cards { get; set; }
        
        public Player(int numberOfCards, GameObject panel)
        {
            NumberOfCards = numberOfCards;
            Panel = panel;
            Cards = new List<Card>();
        }
    }
}
