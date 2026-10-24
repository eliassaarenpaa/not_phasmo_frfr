using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct AnomalyEffect
{
    public AnomalyType type;
    public AnomalyEffectType effect;

    public GameObject GetTargetVersion(AnomalyObject o)
    {
        switch (type)
        {
            case AnomalyType.ObjectChanged:
                return o.changed_ver != null ? o.changed_ver.gameObject : null;
            case AnomalyType.ObjectMoved:
                return o.moved_ver != null ? o.moved_ver.gameObject : null;
            case AnomalyType.UnknownEntity:
                return o.entity_ver != null ? o.entity_ver.gameObject : null;
            case AnomalyType.ObjectRemoved:
                return o.removed_ver != null ? o.removed_ver.gameObject : null;
            case AnomalyType.ElectricalDisturbance:
                return o.electrical_ver != null ? o.electrical_ver.gameObject : null;
            default:
                return null;
        }
    }

    public bool Apply(AnomalyObject o)
    {
        if (effect != AnomalyEffectType.PrefabToggle) return false;

        GameObject target = GetTargetVersion(o);
        if (target == null) return false;

        o.original_ver.gameObject.SetActive(false);
        target.SetActive(true);
        return true;
    }

    public void Revert(AnomalyObject o)
    {
        if (effect != AnomalyEffectType.PrefabToggle) return;

        o.original_ver.gameObject.SetActive(true);

        GameObject target = GetTargetVersion(o);
        if (target != null) target.SetActive(false);
    }
}

public class AnomalyObject : MonoBehaviour
{
    public RoomType room;

    public Transform original_ver;
    public Transform moved_ver;
    public Transform changed_ver;
    public Transform entity_ver;
    public Transform removed_ver;
    public Transform electrical_ver;



    public List<AnomalyEffect> effects = new List<AnomalyEffect>();

    public AnomalyEffect GetRandomEffect()
    {
        return effects[UnityEngine.Random.Range(0, effects.Count)];
    }

    void OnEnable() => AnomalySystem.Register(this);

    void OnDisable() => AnomalySystem.Unregister(this);
}
