using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class Menu : MonoBehaviour
{
    [SerializeField] private Button _startButton;
    [SerializeField] private Button _quitButton;

    [Space(10f)]
    [SerializeField] private TextMeshProUGUI _versionText;

    [Space(10f)]
    [SerializeField] private AudioClip _clip;

    private void Start()
    {
        _startButton.onClick.AddListener(() => StartCoroutine(StartButtonHandler()));
        _quitButton.onClick.AddListener(() => StartCoroutine(QuitButtonHandler()));

        _versionText.text = Application.version;
    }

    private IEnumerator StartButtonHandler()
    {
        ButtonAudioClick.PlaySound(_clip);

        yield return new WaitForSeconds(0.025f);

        SceneManager.LoadScene(1);
    }

    private IEnumerator QuitButtonHandler()
    {
        ButtonAudioClick.PlaySound(_clip);

        yield return new WaitForSeconds(0.025f);

#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }
}
