using UnityEngine;
using UnityEngine.SceneManagement; // Make sure this line is included
using UnityEngine.UI;
using System.Collections;

namespace menu
{
    public class GameMenuController : MonoBehaviour
    {
        public Button game1Button;
        public Button game2Button;
        public Button game3Button;
        public Button game1ButtonTxt;
        public Button game2ButtonTxt;
        public Button game3ButtonTxt;

        public AudioClip buttonClip; // The audio clip to play when a button is clicked
        private AudioSource audioSource; // The audio source to play the clip

        private void Start()
        {
            // Get the AudioSource component
            audioSource = GetComponent<AudioSource>();

            // Check if audioSource component is attached
            if (audioSource == null)
            {
                UnityEngine.Debug.LogError("AudioSource component is missing from the GameObject.");
                return; // Exit if no AudioSource is found
            }

            // Add listeners for button clicks
            game1Button.onClick.AddListener(() => PlaySoundAndLoadGame("Baccarat"));
            game2Button.onClick.AddListener(() => PlaySoundAndLoadGame("Blackjack"));
            game3Button.onClick.AddListener(() => PlaySoundAndLoadGame("WarScene"));
            game1ButtonTxt.onClick.AddListener(() => PlaySoundAndLoadGame("Baccarat"));
            game2ButtonTxt.onClick.AddListener(() => PlaySoundAndLoadGame("Blackjack"));
            game3ButtonTxt.onClick.AddListener(() => PlaySoundAndLoadGame("WarScene"));
        }

        private void PlaySoundAndLoadGame(string sceneName)
        {
            // Play the sound if the clip is assigned
            if (buttonClip != null)
            {
                audioSource.PlayOneShot(buttonClip);
            }

            // Start coroutine to wait for sound to finish before loading scene
            StartCoroutine(LoadSceneAfterSound(sceneName));
        }

        private IEnumerator LoadSceneAfterSound(string sceneName)
        {
            // Wait until the audio has finished playing or for a minimum delay
            yield return new WaitForSeconds(buttonClip != null ? buttonClip.length : 0.5f); // Adjust the delay if needed
            SceneManager.LoadScene(sceneName);
        }
    }
}
