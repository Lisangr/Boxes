using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using YG;

public class TextTranslator : MonoBehaviour
{
    private string lang;
    [SerializeField] private Text gameNameText;
    [SerializeField] private Text exitButtonText;
    [SerializeField] private Text rewardButtonText;

    private readonly Dictionary<string, (string gameName, string exitButton, string rewardButton)> translations =
        new Dictionary<string, (string, string, string)>
        {
            { "ru", ("Разрушай кубики", "В меню", "Награда") },
            { "en", ("Destroy Cubes", "To menu", "Award") },
            { "tr", ("Küpleri Yok Et", "Menüde", "Ödül") },
            { "es", ("Destruye Cubos", "en el menú", "Otorgar") },
            { "de", ("Zerstöre Würfel", "Auf der Speisekarte", "Vergeben") }
        };

    private void Awake()
    {
        lang = YandexGame.EnvironmentData.language;
    }

    private void Start()
    {
        if (translations.TryGetValue(lang, out var translation))
        {
            gameNameText.text = translation.gameName;
            exitButtonText.text = translation.exitButton;
            rewardButtonText.text = translation.rewardButton;
        }
        else
        {
            Debug.LogWarning($"Translation for language '{lang}' not found.");
        }
    }
}