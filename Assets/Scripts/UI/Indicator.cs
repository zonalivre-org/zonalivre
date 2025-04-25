using DG.Tweening;
using UnityEngine;

public class Indicator : MonoBehaviour
{
    [SerializeField] private GameObject target;
    [SerializeField] private float offSet = 2.5f;
    void Start()
    {
        if (target == null)
        {
            Debug.LogError("Target not set for Indicator script on " + gameObject.name);
            return;
        }

        SetTarget(target);

        transform.position = new Vector3(target.transform.position.x, target.transform.position.y + offSet, target.transform.position.z);
    }

    void LateUpdate()
    {
        if (target == null)
        {
            Debug.LogError($"{gameObject.name} doesn't have a target to follow");
            return;
        }
        else
        {
            FollowTarget();
        }
    }

    public void SetTarget(GameObject newTarget)
    {
        target = newTarget;
        if (target != null)
        {
            gameObject.name = "> " + target.name + "_Indicator";
            gameObject.transform.SetParent(target.transform, false);
            gameObject.transform.localPosition = Vector3.zero;
        }
    }

    public void SetTarget(GameObject newTarget, float offSet)
    {
        target = newTarget;
        if (target != null)
        {
            gameObject.name = "> " + target.name + "_Indicator";
            gameObject.transform.SetParent(target.transform, false);
            gameObject.transform.localPosition = Vector3.zero;
            this.offSet = offSet;
        }
    }

    private void FollowTarget()
    {
        Vector3 position = new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z);
        transform.position = position;
    }

}