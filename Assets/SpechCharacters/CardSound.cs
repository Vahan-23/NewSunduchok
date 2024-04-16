using System;
using System.Collections.Generic;
using UnityEngine;

public class CardSound : MonoBehaviour
{
    public AudioClip aceSound;
    public AudioClip kingSound;
    private AudioSource audioSource;


    [SerializeField] private List<CardSoundVariants> sounds = new List<CardSoundVariants>();

    private void Awake()
    {
        // Получаем компонент AudioSource или добавляем его, если его нет
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

    }



    // Метод для воспроизведения звука карты
    public void PlayCardSound(int cardValue)
    {
        if (sounds.Count > cardValue)
            audioSource.PlayOneShot(sounds[cardValue].audioVariants[UnityEngine.Random.Range(0, sounds[cardValue].audioVariants.Count)]);

    
        // Используем switch для воспроизведения соответствующего звука в зависимости от значения карты
        //switch (cardValue)
        //{
        //    case CardDeck.CardsNumber.Ace:
        //        if (aceSound != null)
        //        {
        //            audioSource.PlayOneShot(aceSound);
        //        }
        //        else
        //        {
        //            Debug.LogWarning("No audio clip assigned for Ace sound.");
        //        }
        //        break;
        //    case CardDeck.CardsNumber.King:
        //        if (kingSound != null)
        //        {
        //            audioSource.PlayOneShot(kingSound);
        //        }
        //        else
        //        {
        //            Debug.LogWarning("No audio clip assigned for King sound.");
        //        }
        //        break;
        //    default:
        //        // Если для карты не найден звуковой файл, не воспроизводим ничего
        //        break;
        //}
    }
}

[Serializable]
public class CardSoundVariants
{
    public List<AudioClip> audioVariants = new List<AudioClip>(); 
} 