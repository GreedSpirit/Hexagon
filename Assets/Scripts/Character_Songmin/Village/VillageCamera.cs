using Unity.Cinemachine;
using UnityEngine;

public class VillageCamera : MonoBehaviour
{
    [SerializeField] SpriteRenderer _backGround;
    private void Awake()
    {
        CinemachineCamera cam = GetComponent<CinemachineCamera>();
        float y =  ((_backGround.bounds.size.y) / 4f);
        cam.Lens.OrthographicSize = y;
    }
}
