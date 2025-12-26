using UnityEngine;

public class PlayerUIManager : MonoBehaviour
{
    [SerializeField] GameObject _playerStatUI;
    [SerializeField] GameObject _playerTalkUI;
    [SerializeField] GameObject _playerInventoryUI;

    //private void Start()
    //{
    //    OnOffPlayerStatUi(true);        
    //}

    public void OnOffPlayerStatUi(bool readyToShow)
    {
        if(readyToShow)
        {
            _playerStatUI.SetActive(true);            
        }
        else
        {
            _playerStatUI.SetActive(false);            
        }            
    }
    public void OnOffPlayerTalkUi(bool readyToShow)
    {
        if (readyToShow)
        {
            _playerTalkUI.SetActive(true);
        }
        else
        {
            _playerTalkUI.SetActive(false);
        }
    }
    public void OnOffPlayerInventoryUi(bool readyToShow)
    {
        if (readyToShow)
        {
            _playerInventoryUI.SetActive(true);
        }
        else
        {
            _playerInventoryUI.SetActive(false);
        }
    }

}
