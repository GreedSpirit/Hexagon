using System.Collections;
using TMPro;
using UnityEngine;

public class PlayerLevelUpObject : MonoBehaviour
{    
    [SerializeField] TextMeshProUGUI _levelText;
    [SerializeField] GameObject _levelTextObject;

    public void Init(int level)
    {
        _levelText.text = $"·¹º§ {level}";
        StartCoroutine(LifeRoutine());
    }


    IEnumerator LifeRoutine()
    {
        yield return new WaitForSeconds(2f);
        if (_levelTextObject != null)
        {
            Destroy(_levelTextObject.gameObject);
        }
            

        yield return new WaitForSeconds(0.3f);
        Destroy(gameObject);
    }
}
