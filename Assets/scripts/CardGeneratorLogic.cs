using UnityEngine;

namespace Assets.scripts
{
    public class CardGeneratorLogic : MonoBehaviour
    {
        /// <summary>
        /// Instancia el GameObject carta
        /// </summary>
        /// <param name="card"></param>
        public GameObject GenerateCard(GameObject card)
        {
            GameObject newCard = Instantiate(card);
            newCard.transform.SetParent(transform);
            newCard.transform.localScale = new Vector3(1, 1, 1);

            return newCard;
        }

        // Start is called before the first frame update
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
