using System.Collections;
using TMPro;
using UnityEngine;

public class TurnPopUpObject : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _turnText;
    [SerializeField] float _lifeTime = 1f;

    Coroutine _lifeRoutine;

    public void Show(PhaseType type)
    {
        switch (type)
        {
            case PhaseType.PlayerAct:
                _turnText.text = "플레이어의 턴";
                break;
            case PhaseType.EnemyAct:
                _turnText.text = "상대방의 턴";
                break;
            default:
                return;
        }

        gameObject.SetActive(true);

        if (_lifeRoutine != null)
        {
            StopCoroutine(_lifeRoutine);
        }

        _lifeRoutine = StartCoroutine(LifeRoutine());
    }

    IEnumerator LifeRoutine()
    {
        yield return new WaitForSeconds(_lifeTime);
        _lifeRoutine = null;
        gameObject.SetActive(false);
    }
    private void OnDisable()
    {
        if (_lifeRoutine != null)
        {
            StopCoroutine(_lifeRoutine);
            _lifeRoutine = null;
        }
    }
}
