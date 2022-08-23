using Mike;
using UnityEngine;

public class ChainLightning : MonoBehaviour
{
    [SerializeField] private int bounces = 5;
    [SerializeField] private float speed = 25;
    [SerializeField] private float damage = 5;

    private GameObject[] hitGameObjects = new GameObject[0];
    private GameObject currentTarget;


    //-------------------------------------------


    void Update()
    {
        GetNewTarget();
        MoveOrDealDamage();
    }

    void GetNewTarget()
    {
        if (currentTarget == null && bounces >= 0)
        {
            currentTarget = MikeGameObject.GetClosestTargetWithTag(transform.position, "Enemy", hitGameObjects); 
            if(currentTarget == null) { TurnOffZigZagEffects(); Destroy(gameObject, 1); enabled = false; }
            AddToHitGameObjcets(currentTarget);
            bounces--;
        }
        else if (bounces < 0) { TurnOffZigZagEffects(); Destroy(gameObject, 1); enabled = false; }
    }

    void MoveOrDealDamage()
    {
        if(currentTarget == null) { return; }

        if (Vector2.Distance(transform.position, currentTarget.transform.position) > .5f)
        {
            transform.position = Vector2.MoveTowards(transform.position, currentTarget.transform.position, speed * Time.deltaTime);
            transform.rotation = MikeTransform.Rotation.LookTwards(transform.position, currentTarget.transform.position);
        }
        else 
        {
            currentTarget.GetComponent<Health>().TakeDamage(damage * PlayerPrefs.GetFloat("Damage")); currentTarget = null; 
        }
    }

    void AddToHitGameObjcets(GameObject go)
    {
        GameObject[] temp = hitGameObjects;
        hitGameObjects = new GameObject[hitGameObjects.Length + 1];
        temp.CopyTo(hitGameObjects, 0);
        hitGameObjects[hitGameObjects.Length - 1] = go;
    }

    void TurnOffZigZagEffects()
    {
        foreach (Transform item in transform)
        {
            if (item.GetComponent<ZigZagEffect>() != null) item.GetComponent<ZigZagEffect>().enabled = false;
        }
    }
}
