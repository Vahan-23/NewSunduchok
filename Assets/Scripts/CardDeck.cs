using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.Rendering;
using System.Linq;

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
       // public GameObject gameObject; // Ссылка на игровой объект
    }

    public float offset = 0.5f; // Длина линии
    List<Vector3> generatedPoints = new List<Vector3>();

    [SerializeField] private Transform lineCenterFirst ; // Координаты центра линии


    List<Vector3> DistributePointsOnLine(int numberOfPoints)
    {
        Vector3 lineCenter = lineCenterFirst.position;
        float lineLength = numberOfPoints;
        List<Vector3> points = new List<Vector3>();

        for (int i = 0; i < numberOfPoints; i++)
        {
            float t = i / (float)(numberOfPoints - 1); // Нормализованный параметр от 0 до 1
            Vector3 pointPosition = lineCenter + new Vector3(t * lineLength - lineLength / 2f, 0f, 0f);
            points.Add(pointPosition);
            Debug.Log(pointPosition);
        }
        return points;
    }

    
    public class CardClickHandler : MonoBehaviour
    {
        private Card parentCard;

        public void SetParentCard(Card card)
        {
            parentCard = card;
        }

        public void OnMouseDown()
        {
            if (parentCard != null)
            {
                // Выводим информацию при нажатии на карту
                Debug.Log("Нажата карта: " + parentCard.value + " " + parentCard.suit);
            }
        }
    }

    public List<Card> cards; // Список карт в колоде
    public List<Card> player1Hand; // Рука игрока 1
    public List<Card> player2Hand; // Рука игрока 2
    public List<Card> bittenCards1;
    public List<Card> bittenCards2;// Бита (собранные наборы)
    [SerializeField] private List<Sprite> cardsSprites;
    [SerializeField] private Sprite cardsSpriteBack;

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

        void DealCards()
    {
        player1Hand = new List<Card>();
        player2Hand = new List<Card>();
        int SuteCounts = 0;
        generatedPointIndex = 0;
        for (int i = 0; i < 5; i++)
        {
            if (player1Hand.FindIndex(card => (int) card.value == (int) cards[i].value) == -1)
            {
                SuteCounts++;
            }
            player1Hand.Add(cards[i]);
            player2Hand.Add(cards[i + 5]);
        }
        generatedPoints = DistributePointsOnLine(SuteCounts);

        for (int i = 0; i < 5; i++)
        {
            GameObject player1Card = Instantiate(new GameObject(), GiveCardPosition(player1Hand[i].value, player1Hand, i), Quaternion.identity);
            SpriteRenderer renderer1 = player1Card.AddComponent<SpriteRenderer>();
            renderer1.sprite = player1Hand[i].face;
            renderer1.sortingOrder = 6 - GetCardOrderInLayer(player1Hand[i].value, player1Hand, i);
            player1Card.transform.localScale = Vector3.one * 0.1f;
            player1Card.transform.eulerAngles = new Vector3(0, 0, GetCardOrderInLayer(player1Hand[i].value, player1Hand, i) == 0 ? 0 : GetCardOrderInLayer(player1Hand[i].value, player1Hand, i) % 2 == 0 ? 5 : -5);
            Debug.Log(player1Card.name);
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

            // Устанавливаем родительскую карту для CardClickHandler
            clickHandler.SetParentCard(player1Hand[i]);


            GameObject player2Card = Instantiate(new GameObject(), player2CardPositions[i].position, Quaternion.identity);
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
                Vector3 newPos = generatedPoints[playerCards.IndexOf(requestedCard)];
                List<Card> cards = new List<Card>();
                cards = playerCards;


                // cards.RemoveRange(j, playerCards.Count - j);

                // Считаем количество карт с тем же значением, чтобы определить смещение по Y
                int sameValueCount = cards.Count(card => card.value == number && playerCards.IndexOf(card) < j);

                // Сдвигаем позицию карты по вертикали на 0.5f * sameValueCount
                newPos = new Vector3(newPos.x, newPos.y + 0.5f * sameValueCount, 0);

                return newPos;
            }
            // Если карта не найдена, возвращаем позицию player1CardPositions[j]
            generatedPointIndex++;
            return generatedPoints[generatedPointIndex -1];
        }
        else
        {
            // Если j <= 0, возвращаем позицию player1CardPositions[j]
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
                return cards.Count(card => card.value == number && playerCards.IndexOf(card) < j);
            }
        }

        return 0;
    }



    // Функция, которая обрабатывает ходы игроков
    void PlayerMove(List<Card> currentPlayerHand, List<Card> otherPlayerHand, int requestedValue)
    {
        // Пример хода игрока
        // Проверяем, есть ли у противника карта с запрошенным значением
        for (int i = 0; i < 4; i++)
        {


            Card requestedCard = otherPlayerHand.Find(card => card.value == (CardsNumber)requestedValue);

            if (requestedCard != null)
            {
                // Перемещаем карту из руки противника в руку текущего игрока
                currentPlayerHand.Add(requestedCard);
                otherPlayerHand.Remove(requestedCard);

                // Добавляем проверку на биту (если набор собран)
                CheckForBittenSets(currentPlayerHand, (CardsNumber)requestedValue);
            }
            else
            {
                // Обработка случая, когда противник не имеет нужной карты
                // Можете добавить свою логику для таких ситуаций
            }
        }
    }

    // Проверка наличия биты (набора карт)
    void CheckForBittenSets(List<Card> playerHand, CardsNumber number)
    {
        int cardsCount = 0;
        for (int i = 0; i < 4; i++)
        {
            Card requestedCard = playerHand.Find(card => card.value == number);

            if (requestedCard != null)
            {
                cardsCount++;
            }

        }
        if (cardsCount == 4)
        {
            for (int i = 0; i < 4; i++)
            {
                Card requestedCard = playerHand.Find(card => card.value == number);
                bittenCards1.Add(requestedCard);
                player1Hand.Remove(requestedCard);
            }
        }
    }
}


