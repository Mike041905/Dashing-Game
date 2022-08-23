using UnityEngine;

public class Crate : MonoBehaviour
{
    [SerializeField] private GameObject destructionEffect;
    [SerializeField] private Loot[] lootTable;

    [System.Serializable]
    public struct Loot
    {
        public GameObject item;
        public float weight;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Enemy")) { BreakCreate(); }
    }

    void BreakCreate()
    {
        Instantiate(lootTable[Mike.MikeRandom.RandomIntByWeights(GetAllLootWeights())].item, transform.position, Quaternion.identity);
        if(destructionEffect != null) Instantiate(destructionEffect, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    float[] GetAllLootWeights()
    {
        float[] output = new float[lootTable.Length];

        for (int i = 0; i < lootTable.Length; i++)
        {
            output[i] = lootTable[i].weight;
        }

        return output;
    }
}
