using UnityEngine;

public class PlayerUIManager : MonoBehaviour
{
    [SerializeField] GameObject _playerExpBar;
    [SerializeField] GameObject _playerHpBar;
    [SerializeField] GameObject _playerHpText;    
    [SerializeField] GameObject _playerDefenseText;    
    [SerializeField] GameObject _playerShieldText;    
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
            _playerDefenseText.SetActive(true);
            _playerShieldText.SetActive(true);
            _playerLevelText.SetActive(true);
        }
        else
        {
            _playerExpBar.SetActive(false);
            _playerHpBar.SetActive(false);
            _playerHpText.SetActive(false);
            _playerDefenseText.SetActive(false);
            _playerShieldText.SetActive(false);
            _playerLevelText.SetActive(false);
        }            
    }        
}
