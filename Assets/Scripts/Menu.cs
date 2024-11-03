using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace menu {
    public class GameMenuController : MonoBehaviour
    {
        public Button game1Button;
        public Button game2Button;
        public Button game3Button;

        public Button game1ButtonTxt;
        public Button game2ButtonTxt;
        public Button game3ButtonTxt;

        private void Start()
        {
            // Add listeners for button clicks
            game1Button.onClick.AddListener(() => LoadGame("Baccarat"));
            game2Button.onClick.AddListener(() => LoadGame("Blackjack"));
            game3Button.onClick.AddListener(() => LoadGame("WarScene"));
            game1ButtonTxt.onClick.AddListener(() => LoadGame("Baccarat"));
            game2ButtonTxt.onClick.AddListener(() => LoadGame("Blackjack"));
            game3ButtonTxt.onClick.AddListener(() => LoadGame("WarScene"));
        }

        private void LoadGame(string sceneName)
        {
            // Load the specified game scene
            SceneManager.LoadScene(sceneName);
        }
    }
}

