using UnityEngine;

namespace Assets.scripts
{
    public class Card : MonoBehaviour
    {
        public int Number { get; set; }
        public Color Color { get; set; }
        
        public void PrintCardInfo()
        {
            Debug.Log("Number: " + Number + ", Color: " + Color);
        }
    }

    public enum Color
    {
        Yellow = 1,
        Green = 2,
        Red = 3,
        Blue = 4,
        White = 5,
        Multi = 6
    }
}
