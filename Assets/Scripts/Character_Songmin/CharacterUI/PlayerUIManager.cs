using UnityEngine;

public class PlayerUIManager : MonoBehaviour
{
    [SerializeField] GameObject _playerStatUI;

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

    

}
