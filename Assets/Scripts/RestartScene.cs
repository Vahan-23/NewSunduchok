using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class RestartGame : MonoBehaviour
{
    void Start()
    {
        // Получаем ссылку на компонент кнопки
        Button restartButton = GetComponent<Button>();

        // Привязываем метод RestartScene к событию нажатия кнопки
        restartButton.onClick.AddListener(RestartScene);
    }

    void RestartScene()
    {
        // Получаем индекс текущей сцены
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        // Перезагружаем текущую сцену
        SceneManager.LoadScene(currentSceneIndex);
    }
}
