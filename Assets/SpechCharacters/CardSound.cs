using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class CardSound : MonoBehaviour
{
    public AudioClip YourAudioClip { get; set; }
    private AudioSource audioSource;
    private int lastRandomIndex = -1;
    [SerializeField] private AudioClip FalseSound;
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

    public void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }
    
    public void PlayFalseSound()
    {
        audioSource.PlayOneShot(FalseSound);
    }

    // Метод для воспроизведения звука карты
    public AudioClip GiveCardSound(int cardValue)
    {
        AudioClip selectedClip = null;

        if (sounds.Count > cardValue && sounds[cardValue].audioVariants.Count > 0)
        {
            int randomIndex;

            // Генерируем случайный индекс, исключая последний использованный индекс
            do
            {
                randomIndex = UnityEngine.Random.Range(0, sounds[cardValue].audioVariants.Count);
            } while (randomIndex == lastRandomIndex);

            selectedClip = sounds[cardValue].audioVariants[randomIndex];
            // audioSource.PlayOneShot(selectedClip);

            // Сохраняем последний использованный индекс
            lastRandomIndex = randomIndex;
        }

        return selectedClip;
    }

}

[Serializable]
public class CardSoundVariants
{
    public List<AudioClip> audioVariants = new List<AudioClip>(); 
} 