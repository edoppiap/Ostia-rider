using UnityEngine;

public class CollisionEffects : MonoBehaviour
{
    public GameObject hitEffectPrefab;

    private void OnCollisionEnter(Collision collision)
    {
        Instantiate(hitEffectPrefab, collision.GetContact(0).point, Quaternion.identity);
    }
}
