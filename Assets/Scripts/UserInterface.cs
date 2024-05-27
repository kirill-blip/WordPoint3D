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

    private GameManager _gameManager;

    public event UnityAction GoButtonPressed;
    public event UnityAction NextWordButtonPressed;

    private void Start()
    {
        _gameManager = FindObjectOfType<GameManager>();

        // _gameManager.WordAssembled += HandleWordAssembled;
        // _gameManager.WordAssembledIncorrect += HandleWordAssembledIncorrect;
        // _gameManager.WordsOvered += HandleWordsOvered;
        
        _goButton.onClick.AddListener(() =>
        {
            GoButtonPressed?.Invoke();
            _goButton.gameObject.SetActive(false);
        });

        _nextWordButton.onClick.AddListener(() =>
        {
            NextWordButtonPressed?.Invoke();
            _goButton.gameObject.SetActive(true);
            _nextWordButton.gameObject.SetActive(false);
        });

        _restartButton.onClick.AddListener(() =>
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        });

        _menuButton.onClick.AddListener(() =>
        {
            SceneManager.LoadScene(0);
        });
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

    private IEnumerator ActivatePanel(Color color)
    {
        _image.gameObject.SetActive(true);
        _image.color = color;

        yield return new WaitForSeconds(0.2f);

        _image.gameObject.SetActive(false);
    }
}
