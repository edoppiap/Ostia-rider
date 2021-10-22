using UnityEngine;

public class CollisionEffects : MonoBehaviour
{
    public GameObject hitEffectPrefab;

    public enum IgnoreTag { Player, Ground }
    public IgnoreTag ignoreTag;
    //public string ignoreTag;

    private void OnCollisionEnter(Collision collision)
    {
        if(!collision.rigidbody.CompareTag(ignoreTag.ToString()))
            Instantiate(hitEffectPrefab, collision.GetContact(0).point, Quaternion.identity);
    }
}
