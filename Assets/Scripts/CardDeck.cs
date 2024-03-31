using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.Rendering;
using System.Linq;
using UnityEngine.Events;

public class CardDeck : MonoBehaviour
{
    public enum Suit { Hearts, Diamonds, Clubs, Spades };
    public enum CardsNumber
    {
        Two,
        Three,
        Four,
        Five,
        Six,
        Seven,
        Eight,
        Nine,
        Ten,
        Jack,
        Queen,
        King,
        Ace
    };

    [System.Serializable]
    public class Card
    {
        public CardsNumber value; // Значение карты (2-10, валет - 11, дама - 12, король - 13, туз - 14)
        public Suit suit; // Масть карты
        public Sprite face; // Изображение карты лицом вверх
        public Sprite back; // Задняя сторона карты
        public int posPointIndex;
        // public GameObject gameObject; // Ссылка на игровой объект
    }

    List<Vector3> generatedPoints = new List<Vector3>();

    private int deckUpperCardIndex = 0;
    [SerializeField] private Transform lineCenterFirst; // Координаты центра линии


    List<Vector3> DistributePointsOnLine(int numberOfPoints)
    {

        Vector3 lineCenter = lineCenterFirst.position;
        List<Vector3> points = new List<Vector3>();
        float offset = 1.4566f;  // Длина линии
        
        float lineLength = offset * (numberOfPoints - 1);
        for (int i = 0; i < numberOfPoints; i++)
        {
            float t = (float)(i / (float)(numberOfPoints - 1)); 
            Vector3 pointPosition = lineCenter + new Vector3((t - 1/2) * lineLength - lineLength/2, 0f, 0f);

            points.Add(pointPosition);
        }
        Debug.Log(points.Count + " :pointsCount");
        Debug.Log(numberOfPoints + " :numberOfPoints");
        return points;
    }

    public class CardClickHandler : MonoBehaviour
    {
        private Card parentCard;
        private List<Card> player1Hand;
        private List<Card> player2Hand;
        //public UnityEvent<int> OnClickedOnCard;
        private CardDeck cardDeck;

        public void Initialize(Card parentCard, List<Card> player1Hand, List<Card> player2Hand, CardDeck cardDeck)
        {
            this.parentCard = parentCard;
            this.player1Hand = player1Hand;
            this.player2Hand = player2Hand;
            this.cardDeck = cardDeck;
        }

        public void OnMouseDown()
        {
            if (parentCard != null)
            {
                //OnClickedOnCard.Invoke((int)parentCard.value);
                cardDeck.PlayerMove(player1Hand, player2Hand, (int)parentCard.value);
               // Debug.Log("Нажата карта: " + parentCard.value + " " + parentCard.suit);
            }
            else
            {
                Debug.LogError("parentCard is null in OnMouseDown");
            }
        }
    }

    private List<CardClickHandler> cardObjs = new List<CardClickHandler>();
    public List<Card> cards; // Список карт в колоде
    public List<Card> player1Hand = null; // Рука игрока 1
    public List<Card> player2Hand; // Рука игрока 2
    public List<Card> bittenCards1;
    private int bittenCount1 = 0;
    private int bittenCount2 = 0;
    public List<Card> bittenCards2;// Бита (собранные наборы)
    [SerializeField] private List<Sprite> cardsSprites;
    [SerializeField] private Sprite cardsSpriteBack;
    [SerializeField] private Animator _DogAnimator;

    // [SerializeField] private List<Transform> cardPositions;

    [SerializeField] private List<Transform> player1CardPositions;
    [SerializeField] private List<Transform> player2CardPositions;


    void Start()
    {
        GenerateDeck();
        ShuffleDeck();
        DealCards();
        // Начало игры, где игроки спрашивают друг у друга карты и проверяют на биту
    }

    void GenerateDeck()
    {
        cards = new List<Card>();

        for (int i = 0; i <= 12; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                Card newCard = new Card();
                newCard.value = (CardsNumber)i;
                newCard.suit = (Suit)j;
                newCard.face = cardsSprites[i * 4 + j];
                newCard.back = cardsSpriteBack;
                // newCard.gameObject = Instantiate(new GameObject(), Vector3.zero, Quaternion.identity); // Создаем пустой игровой объект
                // Здесь вы можете добавить изображения для лицевой и задней стороны каждой карты
                // Например, newCard.face = Resources.Load<Sprite>("Sprites/" + i + "_" + (Suit)j);
                // newCard.back = Resources.Load<Sprite>("Sprites/back");
                cards.Add(newCard);
            }
        }
    }

    void ShuffleDeck()
    {

        for (int i = 0; i < cards.Count; i++)
        {
            Card temp = cards[i];
            int randomIndex = Random.Range(i, cards.Count);
            cards[i] = cards[randomIndex];
            cards[randomIndex] = temp;
        }
    }

    public class PulseEffect : MonoBehaviour
    {
        public float pulseSpeed = 0.4f;  // Скорость пульсации
        public float pulseScale = 0.4f;  // Максимальный масштаб во время пульсации

        private Vector3 initialScale;

        void Start()
        {
            initialScale = transform.localScale;
        }

        void Update()
        {
            float scale = 1.0f - Mathf.PingPong(Time.time * pulseSpeed, pulseScale);
            transform.localScale = initialScale * scale;
        }
    }

    int suteCounts = 0;
    void CreateCards(int Count)
    {
        DestroyOldCards();

        int SuteCounts = 0;

        generatedPointIndex = 0;
        for (int i = 0; i < Count; i++)
        {
            if (player1Hand.FindIndex(card => (int)card.value == (int)player1Hand[i].value) == -1)
            {
                SuteCounts++;
            }
        }
        generatedPoints = DistributePointsOnLine(suteCounts);


        for (int i = 0; i < Count; i++)
            CreateCard(i);

    }
    void DestroyOldCards()
    {
        for (int i = 0; i < cardObjs.Count; i++)
            Destroy(cardObjs[i].gameObject);

        cardObjs.Clear();
    }
    void CreateCard(int i)
    {
        GameObject player1Card = new GameObject();
        player1Card.transform.position = GiveCardPosition(player1Hand[i].value, player1Hand, i);
        SpriteRenderer renderer1 = player1Card.AddComponent<SpriteRenderer>();
        renderer1.sprite = player1Hand[i].face;
        renderer1.sortingOrder = 6 - GetCardOrderInLayer(player1Hand[i].value, player1Hand, i);
        player1Card.transform.localScale = Vector3.one * 0.1f;
        player1Card.transform.eulerAngles = new Vector3(0, 0, GetCardOrderInLayer(player1Hand[i].value, player1Hand, i) == 0 ? 0 : GetCardOrderInLayer(player1Hand[i].value, player1Hand, i) % 2 == 0 ? 5 : -5);
        BoxCollider2D collider = player1Card.AddComponent<BoxCollider2D>();



        PulseEffect pulseEffect = player1Card.AddComponent<PulseEffect>();

        // Настройка параметров пульсации, если необходимо
        pulseEffect.pulseSpeed = 0.105f;
        pulseEffect.pulseScale = 0.04f;


        collider.size = new Vector2(12, 19);
        collider.enabled = true;


        //player1Hand[i].gameObject.name = "Player1Card" + i; // Устанавливаем осмысленное имя
        player1Card.AddComponent<CardClickHandler>();

        CardClickHandler clickHandler = player1Card.GetComponent<CardClickHandler>();
        if (clickHandler == null)
        {
            clickHandler = player1Card.AddComponent<CardClickHandler>();
        }
        clickHandler.Initialize(player1Hand[i], player1Hand, player2Hand, this);

        cardObjs.Add(clickHandler);
    }
    void DealCards()
    {

        player1Hand = new List<Card>();
        player2Hand = new List<Card>();
        suteCounts = 0;
        int SuteCounts = 0;
        generatedPointIndex = 0;
        for (int i = 0; i < 4; i++)
        {
            if (player1Hand.FindIndex(card => (int)card.value == (int)cards[i].value) == -1)
            {
                SuteCounts++;
            }
            player1Hand.Add(cards[i]);
            player2Hand.Add(cards[i + 4]);
        }
        deckUpperCardIndex = 8;
        generatedPoints = DistributePointsOnLine(SuteCounts);
        suteCounts = SuteCounts;
        DestroyOldCards();

        for (int i = 0; i < 4; i++)
        {
            CreateCard(i);

            GameObject player2Card = new GameObject();
            player2Card.transform.position = player2CardPositions[i].position;
            SpriteRenderer renderer2 = player2Card.AddComponent<SpriteRenderer>();
            renderer2.sprite = player2Hand[i].back;
            renderer2.sortingOrder = 1;
            player2Card.transform.localScale = Vector3.one * 0.05f;
        }
    }

    int generatedPointIndex = 0;


    private Vector3 GiveCardPosition(CardsNumber number, List<Card> playerCards, int j)
    {

        if (j > 0)
        {
            // Ищем карту с заданным значением в руке игрока
            Card requestedCard = playerCards.Find(card => card.value == number);

            if (requestedCard != null && requestedCard != playerCards[j])
            {
                // Если карта найдена, возвращаем позицию этой карты в массиве позиций
                Vector3 newPos = generatedPoints[requestedCard.posPointIndex]; //[playerCards.IndexOf(requestedCard)];  // minchev irar vra qarer kan helac , ira indexy != generatedPoints-in 
                playerCards[j].posPointIndex = requestedCard.posPointIndex;
                List<Card> cards = new List<Card>();
                cards = playerCards;


                // cards.RemoveRange(j, playerCards.Count - j);

                // Считаем количество карт с тем же значением, чтобы определить смещение по Y
                int sameValueCount = cards.Count(card => card.value == number && playerCards.IndexOf(card) < j);

                // Сдвигаем позицию карты по вертикали на 0.5f * sameValueCount
                newPos = new Vector3(newPos.x, newPos.y + 0.5f * sameValueCount, 0);

                return newPos;
            }

            playerCards[j].posPointIndex = generatedPointIndex;
            generatedPointIndex++;
            return generatedPoints[generatedPointIndex - 1];

            // Если карта не найдена, возвращаем позицию player1CardPositions[j]
            //Debug.Log(generatedPointIndex);
            //Debug.Log("generated points"+generatedPoints.Count);
        }
        else
        {
            // Если j <= 0, возвращаем позицию player1CardPositions[j]
            //Debug.Log(generatedPointIndex);
            generatedPointIndex++;

            return generatedPoints[generatedPointIndex - 1];
        }
    }

    private int GetCardOrderInLayer(CardsNumber number, List<Card> playerCards, int j)
    {

        if (j > 0)
        {
            Card requestedCard = playerCards.Find(card => card.value == number);

            if (requestedCard != null && requestedCard != playerCards[j])
            {
                return playerCards.Count(card => card.value == number && playerCards.IndexOf(card) < j);
            }
        }

        return 0;
    }

    public void PlayerMove(List<Card> currentPlayerHand, List<Card> otherPlayerHand, int requestedValue)
    {
        bool foundRequestedCard = false;

        for (int i = 0; i < 4; i++)
        {
            Card requestedCard = otherPlayerHand.Find(card => card.value == (CardsNumber)requestedValue);

            if (requestedCard != null)
            {
                currentPlayerHand.Add(requestedCard);
                otherPlayerHand.Remove(requestedCard);

                foundRequestedCard = true;

                CheckForBittenSets(currentPlayerHand, (CardsNumber)requestedValue, bittenCards1, suteCounts);
                _DogAnimator.SetBool("GiveCard", true);
                
                Debug.Log("Игрок: найдена запрошенная карта: " + requestedCard.value);
            }       
            else
            {
                Debug.Log("Игрок: Запрошенная карта не найдена в руке противника.");
             
            }
        }

        if (!foundRequestedCard)
        {
            if (deckUpperCardIndex < cards.Count)
            {
                if (currentPlayerHand.FindIndex(card => card.value == cards[deckUpperCardIndex].value) == -1 && currentPlayerHand == player1Hand)
                    suteCounts++;

                currentPlayerHand.Add(cards[deckUpperCardIndex]);
                CheckForBittenSets(currentPlayerHand, cards[deckUpperCardIndex].value, bittenCards1, suteCounts);

                deckUpperCardIndex++;
                Debug.Log("Игрок взял верхнюю карту из колоды: " + cards[deckUpperCardIndex - 1].value);
            }
            else
            {
                deckUpperCardIndex = cards.Count;
                Debug.Log("Игрок: Верхний индекс карты достиг конца колоды.");
            }

            //PlayerMove(otherPlayerHand, currentPlayerHand, (int)otherPlayerHand[Random.Range(0, otherPlayerHand.Count)].value);
            OpponentMove(otherPlayerHand, currentPlayerHand);
        }

        CreateCards(currentPlayerHand.Count);
    }





    public void OpponentMove(List<Card> opponentHand, List<Card> playerHand)
    {
        bool foundRequestedOpCard = false;

        if (opponentHand.Count > 0)
        {
            int cardCou = 0;
            int randomIndex = Random.Range(0, opponentHand.Count);
            Card selectedCard = opponentHand[randomIndex]; // Запоминаем выбранную карту противника

            for (int j = 0; j < 4; j++)
            {
                // Проверяем наличие выбранной карты у игрока
                Card requestedCard = playerHand.Find(card => card.value == selectedCard.value);

                if (requestedCard != null)
                {
                    cardCou++;

                    // Если карта найдена, противник берет её у игрока
                    opponentHand.Add(requestedCard);
                    playerHand.Remove(requestedCard);

                    _DogAnimator.SetBool("GetCard", true);
                    Debug.Log("Противник получил карту " + requestedCard.value + " от игрока. Попытка номер " + cardCou);

                    for (int i = 0; i < playerHand.Count; i++)
                    {
                        playerHand[i].posPointIndex = i;
                    }

                    // Проверяем наличие биты после хода противника
                    foundRequestedOpCard = true;
                    CheckForBittenOpponentSets(opponentHand, requestedCard.value);

                }

            }
            if (!foundRequestedOpCard)
            {
                // Если у игрока нет запрошенной карты, противник берет карту из колоды
                if (deckUpperCardIndex < cards.Count)
                {
                    opponentHand.Add(cards[deckUpperCardIndex]);
                    deckUpperCardIndex++;
                    Debug.Log("Противник запросил карту " + selectedCard.value + ", но не получил её от игрока. Противник взял карту из колоды.");

                    // Проверяем наличие биты после хода противника
                    CheckForBittenOpponentSets(opponentHand, cards[deckUpperCardIndex - 1].value);
                }
                else
                {
                    Debug.Log("Противник запросил карту " + selectedCard.value + ", но не получил её от игрока. Колода пуста, противник не может взять карту.");
                }
            }
            else
            {
                suteCounts--;
                OpponentMove(opponentHand, playerHand);
            }
        }
        else
        {
            Debug.Log("Противник не имеет карт в руке.");
        }

        CreateCards(playerHand.Count);
    }






    // Проверка наличия биты (набора карт)
    void CheckForBittenSets(List<Card> playerHand, CardsNumber number , List<Card> bittenCards , int SuteCounts = -1)
    {
        List<Card> cardsToRemove = new List<Card>();

        // Находим все карты, которые нужно удалить
        foreach (var card in playerHand)
        {
            if (card.value == number)
            {
                cardsToRemove.Add(card);
            }
        }

        // Если найдено 4 карты с таким значением, удаляем их из руки игрока
        if (cardsToRemove.Count == 4)
        {
            if(SuteCounts != -1)
            suteCounts--;

            foreach (var card in cardsToRemove)
            {
                bittenCards1.Add(card);
                bittenCount1++;
                playerHand.Remove(card);
            }

            // Обновляем индексы оставшихся карт в playerHand
            for (int i = 0; i < playerHand.Count; i++)
            {
                playerHand[i].posPointIndex = i;
            }
        }
    }


    void CheckForBittenOpponentSets(List<Card> opponentHand, CardsNumber number)
    {
        List<Card> cardsToRemove = new List<Card>();

        // Находим все карты, которые нужно удалить
        foreach (var card in opponentHand)
        {
            if (card.value == number)
            {
                cardsToRemove.Add(card);
            }
        }

        // Если найдено 4 карты с таким значением, удаляем их из руки игрока
        if (cardsToRemove.Count == 4)
        {
            

            foreach (var card in cardsToRemove)
            {
                bittenCards2.Add(card);
                bittenCount2++;
                opponentHand.Remove(card);
            }

            // Обновляем индексы оставшихся карт в playerHand
            for (int i = 0; i < opponentHand.Count; i++)
            {
                opponentHand[i].posPointIndex = i;
            }
        }
    }

}
