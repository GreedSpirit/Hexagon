using UnityEngine;

public class PlayerUIManager : MonoBehaviour
{
    [SerializeField] GameObject _playerExpBar;
    [SerializeField] GameObject _playerHpBar;
    [SerializeField] GameObject _playerHpText;    
    [SerializeField] GameObject _playerLevelText;    

    private void Start()
    {
        OnOffPlayerStatUi(true);
    }

    public void OnOffPlayerStatUi(bool readyToShow)
    {
        if(readyToShow)
        {
            _playerExpBar.SetActive(true);
            _playerHpBar.SetActive(true);
            _playerHpText.SetActive(true);
            _playerLevelText.SetActive(true);
        }
        else
        {
            _playerExpBar.SetActive(false);
            _playerHpBar.SetActive(false);
            _playerHpText.SetActive(false);
            _playerLevelText.SetActive(false);
        }            
    }        
}
