using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UserInterface : MonoBehaviour
{
    [SerializeField] private Button _goButton;
    [SerializeField] private Button _nextWordButton;

    [SerializeField] private Image _image;
    [SerializeField] private Color _green;
    [SerializeField] private Color _red;

    [SerializeField] private GameObject _endPanel;

    [SerializeField] private Button _restartButton;
    [SerializeField] private Button _menuButton;
    [SerializeField] private Button _helpButton;

    [SerializeField] private AudioClip _clip;

    public event UnityAction GoButtonPressed;
    public event UnityAction NextWordButtonPressed;
    public event UnityAction HelpButtonPressed;

    private void Start()
    {
        _goButton.onClick.AddListener(() =>
        {
            ButtonAudioClick.PlaySound(_clip);

            GoButtonPressed?.Invoke();
            _goButton.gameObject.SetActive(false);
        });

        _nextWordButton.onClick.AddListener(() =>
        {
            ButtonAudioClick.PlaySound(_clip);

            NextWordButtonPressed?.Invoke();
            _nextWordButton.gameObject.SetActive(false);
        });

        _helpButton.onClick.AddListener(() =>
        {
            ButtonAudioClick.PlaySound(_clip);

            HelpButtonPressed?.Invoke();
        });

        _restartButton.onClick.AddListener(() =>
        {
            StartCoroutine(RestartButtonHandler());
        });

        _menuButton.onClick.AddListener(() =>
        {
            StartCoroutine(MenuButtonHandler());
        });
    }

    private IEnumerator RestartButtonHandler()
    {
        ButtonAudioClick.PlaySound(_clip);

        yield return new WaitForSeconds(.025f);
        
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private IEnumerator MenuButtonHandler()
    {
        ButtonAudioClick.PlaySound(_clip);

        yield return new WaitForSeconds(.025f);

        SceneManager.LoadScene(0);
    }

    public void HandleWordsOvered()
    {
        _nextWordButton.gameObject.SetActive(false);

        _endPanel.gameObject.SetActive(true);
    }

    public void HandleWordAssembledIncorrect()
    {
        StartCoroutine(ActivatePanel(_red));
    }

    public void HandleWordAssembled()
    {
        _nextWordButton.gameObject.SetActive(true);

        StartCoroutine(ActivatePanel(_green));
    }

    public void ActivateGoButton()
    {
        _goButton.gameObject.SetActive(true);
    }

    private IEnumerator ActivatePanel(Color color)
    {
        _image.gameObject.SetActive(true);
        _image.color = color;

        yield return new WaitForSeconds(0.2f);

        _image.gameObject.SetActive(false);
    }
}
