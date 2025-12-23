using UnityEngine;

public class PlayerUIManager : MonoBehaviour
{
    [SerializeField] GameObject _playerStatUI;
    [SerializeField] GameObject _playerTalkUI;

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


}
