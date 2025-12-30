using Unity.Cinemachine;
using UnityEngine;

public class VillageCamera : MonoBehaviour
{
    [SerializeField] SpriteRenderer _backGround;
    private void Start()
    {
        CinemachineCamera cam = GetComponent<CinemachineCamera>();
        if (cam.Follow != Player.Instance.transform)
        {
            cam.Follow = Player.Instance.transform;
        }

        float y =  ((_backGround.bounds.size.y) / 4f);
        cam.Lens.OrthographicSize = y;
    }
}
