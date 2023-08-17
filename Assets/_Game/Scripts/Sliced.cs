using System.Collections.Generic;
using UnityEngine;

public class Sliced : MonoBehaviour
{
    [SerializeField] List<Rigidbody> _parts;
    [SerializeField] int _scores = 10;
    const float EXPLOSION_FORCE = 555f;
    const float EXPLOSION_RADIUS = 555f;  
    
    public void Explode()
    {
        EventBus.OnScoresEarned.Invoke(_scores);
        foreach (Rigidbody part in _parts)
        {
            part.isKinematic = false;
            part.AddExplosionForce(EXPLOSION_FORCE, Random.insideUnitSphere, EXPLOSION_RADIUS);
        }
    }
}
