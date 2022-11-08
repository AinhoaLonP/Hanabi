using Assets.scripts;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Game : MonoBehaviour
{
    public int numberOfPlayers;
    public bool multiColor;
    public int hints = 8;
    public int errors = 0;

    public GameObject gameCanvas;
    public GameObject card;

    public GameObject bottomPanel;
    public GameObject upperPanel;
    public GameObject leftPanel;
    public GameObject rightPanel;
    public GameObject upperLeftPanel;
    public GameObject upperRightPanel;

    public GameObject optionsPanel;
    public Button btDiscard;
    public Button btPlay;

    private List<Card> deck;
    private Card[,] playedCards;
    private List<Card> discardedCards;
    private GameObject selectedCard;

    // Start is called before the first frame update
    void Start()
    {
        deck = FillDeck(multiColor ? 60 : 50); //Si es partida con multicolor, el mazo tiene 60 cartas. Si no, 50
        playedCards = new Card[multiColor ? 6 : 5, 5]; //Array multidimensional de 6x5 si hay multicolor o 5x5 si no
        discardedCards = new List<Card>();

        int numberOfCards;
        if (numberOfPlayers < 4) numberOfCards = 5; //Si hay 2 o 3 jugadores, se reparten 5 cartas a cada uno
        else numberOfCards = 4; //Si hay 4 o 5, 4 cartas a cada uno

        List<Player> players = new List<Player>();
        for (int i = 0; i < numberOfPlayers; i++)
        {
            GameObject assignedPanel = AssignPanel(i); //Se le asigna un panel a cada jugador, según el número de jugadores
            players.Add(new Player(numberOfCards, assignedPanel));
        }

        Deal(players); //Se reparte la mano inicial     

        btDiscard.onClick.AddListener(delegate { DiscardCard(selectedCard, players[0]); }); //TODO cambiar la forma de obtener el segundo parámetro
        btPlay.onClick.AddListener(delegate { PlayCard(selectedCard, players[0]); });
    }

    /// <summary>
    /// Asigna un panel a cada jugador. La colocación dependerá del número de jugadores.
    /// </summary>
    /// <param name="playerIndex"></param> Índice del jugador
    /// <returns></returns>
    GameObject AssignPanel(int playerIndex)
    {
        GameObject assignedPanel;
        if (playerIndex == 0) assignedPanel = bottomPanel; //El jugador 0 siempre es el de abajo
        else
        {
            if (numberOfPlayers == 2) assignedPanel = upperPanel; //Si hay 2 jugadores, el otro se coloca arriba
            else if (numberOfPlayers == 3) //Si hay 3, se colocan abajo, arriba y derecha
            {
                if (playerIndex == 1) assignedPanel = upperPanel;
                else assignedPanel = rightPanel;
            }
            else if (numberOfPlayers == 4) //Con 4: abajo, izquierda, arriba, derecha
            {
                if (playerIndex == 1) assignedPanel = leftPanel;
                else if (playerIndex == 2) assignedPanel = upperPanel;
                else assignedPanel = rightPanel;
            }
            else //Con 5: abajo, izquierda, dos arriba, derecha
            {
                if (playerIndex == 1) assignedPanel = leftPanel; 
                else if (playerIndex == 2) assignedPanel = upperLeftPanel;
                else if (playerIndex == 3) assignedPanel = upperRightPanel;
                else assignedPanel = rightPanel;
            }
        }
        return assignedPanel;
    }

    /// <summary>
    /// Rellena el mazo al principio de la partida con todas las cartas del juego
    /// </summary>
    /// <param name="totalNumberOfCards">Número total de cartas (50 para partidas normales, 60 con multicolor)</param>
    /// <returns>El mazo completo</returns>
    List<Card> FillDeck(int totalNumberOfCards)
    {
        List<Card> fullDeck = new List<Card>();

        for (int i = 0; i < totalNumberOfCards; i++)
        {
            Card card = new Card();
            card.Color = (Assets.scripts.Color)(i / 10) + 1;

            if (i % 10 <= 2) card.Number = 1; //Para cada color, las tres primeras cartas son unos
            else if (i % 10 <= 4) card.Number = 2; //Las dos siguientes son doses
            else if (i % 10 <= 6) card.Number = 3; //Las dos siguientes son treses
            else if (i % 10 <= 8) card.Number = 4; //Las dos siguientes son cuatros
            else card.Number = 5; //La última de cada color es un cinco

            fullDeck.Add(card);
        }

        return fullDeck;
    }

    /// <summary>
    /// A cada jugador le asigna su mano inicial
    /// </summary>
    /// <param name="players">Jugadores de la partida</param>
    void Deal(List<Player> players)
    {
        foreach (Player player in players)
        {
            for (int i = 0; i < player.NumberOfCards; i++)
            {
                DrawCard(player);
            }
        }
    }

    /// <summary>
    /// Roba, con índice entre 0 y el número de cartas
    /// <param name="player">El jugador que roba</param>
    /// </summary>
    void DrawCard(Player player) {
        int indexOfCardToDraw = Random.Range(0, deck.Count - 1);
        Card drawnCard = deck[indexOfCardToDraw];
        player.Cards.Add(drawnCard);
        deck.RemoveAt(indexOfCardToDraw);

        GameObject cardGO = player.Panel.GetComponent<CardGeneratorLogic>().GenerateCard(card); //Instanciar un GameObject carta
        cardGO.GetComponent<Card>().Color = drawnCard.Color;
        cardGO.GetComponent<Card>().Number = drawnCard.Number;

        //Le ponemos a la carta un listener para que se llame al método cuando cliquemos en ella.
        //Se debe hacer con un listener porque los prefabs no admiten el onClick(), y con el delegate para poder pasarle un parámetro
        if (player.Panel == bottomPanel) //TODO mejorar esta condición
            cardGO.GetComponentInChildren<Button>().onClick.AddListener(delegate{ SelectCard(cardGO); });
        else
        {
            //TODO activar panel de elegir color o número para pista
        }
    }

    /// <summary>
    /// Ponemos la carta como seleccionada
    /// </summary>
    /// <param name="cardGO">Carta que seleccionamos</param>
    void SelectCard(GameObject cardGO)
    {
        ActivateOptionsPanel();
        selectedCard = cardGO;
    }

    /// <summary>
    /// Activa el panel de opciones (jugar o descartar)
    /// </summary>
    void ActivateOptionsPanel()
    {
        optionsPanel.SetActive(true);
    }

    /// <summary>
    /// El jugador juega una carta
    /// </summary>
    /// <param name="cardToPlayGO">El gameobject de la carta que va a jugar</param>
    /// <param name="player">El jugador que juega</param>
    /// <returns></returns>
    void PlayCard(GameObject cardToPlayGO, Player player)
    {
        Card cardToPlay = cardToPlayGO.GetComponent<Card>();
        player.Cards.Remove(cardToPlay); //Se elimina la carta de la mano del jugador
        //TODO de momento lo eliminamos, en el futuro habrá que moverlo al centro
        Destroy(cardToPlayGO.gameObject); //Eliminamos el gameobject de la carta

        //Se comprueba que el espacio en las cartas jugadas no está ocupado (se resta 1 porque el índice empieza en 0 y los números y colores no)
        if (playedCards[(int)cardToPlay.Color - 1, cardToPlay.Number - 1] == null
            //Además, la carta jugada debe ser un 1 o la anterior debe estar ya colocada
            && (cardToPlay.Number == 1 || playedCards[(int)cardToPlay.Color - 1, cardToPlay.Number - 2] != null))
        {     
            playedCards[(int)cardToPlay.Color - 1, cardToPlay.Number - 1] = cardToPlay; //Se coloca la carta
            if (cardToPlay.Number == 5) IncrementHints(); //Si se coloca el número 5 se aumenta el número de pistas disponibles

            /*
            foreach (Card card in playedCards)
            {
                if (card != null)
                {
                    Debug.Log("------------ carta jugada: ");
                    card.PrintCardInfo();
                }
            }
            */
        }
        else
        {
            discardedCards.Add(cardToPlay);
            IncrementErrors();
        }

        if (deck.Count > 0)
            DrawCard(player);
        else
        {
            //TODO Última ronda
        }
    }

    void IncrementHints()
    {
        if (hints < 8)
            hints++;
    }

    void DecrementHints()
    {
        if (hints > 0)
            hints--;
    }

    void IncrementErrors()
    {
        errors++;
        if (errors >= 3)
        {
            //TODO Perder partida
        }
    }

    /// <summary>
    /// Descarta una carta
    /// </summary>
    /// <param name="cardToDiscardGO">Gameobject de la carta para descartar</param>
    /// <param name="player">Jugador que descarta una carta</param>
    void DiscardCard(GameObject cardToDiscardGO, Player player){
        selectedCard = null; //Deseleccionamos la carta que teníamos seleccionada
        Card cardToDiscard = cardToDiscardGO.GetComponent<Card>();
        discardedCards.Add(cardToDiscard);

        /*
        Debug.Log("Number of discarded cards: " + discardedCards.Count);
        foreach (Card card in discardedCards)
        {
            card.PrintCardInfo();
        }
        */

        //TODO de momento lo eliminamos, en un futuro habrá que moverlo a la pila de descartes
        Destroy(cardToDiscardGO.gameObject); //Eliminamos el gameobject de la carta
        IncrementHints();
        if (deck.Count > 0)
            DrawCard(player);
        else
        {
            //TODO Última ronda
        }
    }
}
