using UnityEngine;
using System.Collections.Generic;

public class MonsterVFXController : MonoBehaviour
{
    [Header("Spawn Point")]
    [SerializeField] private Transform _effectCenter; // 이펙트가 생성될 위치 (몬스터의 배/가슴 쯤?)

    [Header("Common VFX")]
    [SerializeField] private GameObject _hitVfx;
    [SerializeField] private GameObject _healVfx;

    [Header("Status Effect VFX")]
    [SerializeField] private GameObject _burnVfx;
    [SerializeField] private GameObject _despairVfx;
    [SerializeField] private GameObject _poisonVfx;
    [SerializeField] private GameObject _knowledgeVfx;
    [SerializeField] private GameObject _prideVfx;
    [SerializeField] private GameObject _stigmaVfx; 
    [SerializeField] private GameObject _vulnerableVfx; 

    private void Awake()
    {
        if (_effectCenter == null) _effectCenter = transform;
    }

    public void PlayHitEffect()
    {
        SpawnParticle(_hitVfx);
    }

    public void PlayHealEffect()
    {
        SpawnParticle(_healVfx);
    }

    public void PlayStatusEffect(string effectKey)
    {
        GameObject targetVfx = null;

        switch (effectKey)
        {
            case "KeyStatusBurn":
                targetVfx = _burnVfx;
                break;

            case "KeyStatusDespair":
                targetVfx = _despairVfx;
                break;

            case "KeyStatusPoison":
                targetVfx = _poisonVfx;
                break;

            case "KeyStatusKnowledge":
                targetVfx = _knowledgeVfx;
                break;

            case "KeyStatusPride":
                targetVfx = _prideVfx;
                break;
            
            case "KeyStatusStigma":
                targetVfx = _stigmaVfx;
                break;

            case "KeyStatusVulnerable":
                targetVfx = _vulnerableVfx;
                break;
        }

        if (targetVfx != null)
        {
            SpawnParticle(targetVfx);
        }
    }

    private void SpawnParticle(GameObject prefab)
    {
        if (prefab == null) return;

        GameObject obj = Instantiate(prefab, _effectCenter.position, Quaternion.identity);
        
        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();

        foreach(var r in renderers)
        {
            r.sortingOrder = 100;
        }

        Destroy(obj, 2f); 
    }
}