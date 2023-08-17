using UnityEngine;
[RequireComponent(typeof(Rigidbody), typeof(BoxCollider))]
public class Card : MonoBehaviour
{
    public MeshRenderer CardMesh { get; private set; }
    public Rigidbody Rigidbody { get; private set; }
    public BoxCollider BoxCollider { get; private set; }
    private void Awake()
    {
        Rigidbody = GetComponent<Rigidbody>();
        BoxCollider = GetComponent<BoxCollider>();
    }

    public void TurnOver()
    {
        Rigidbody.isKinematic = true;
        BoxCollider.enabled = false;
    }
    private void OnTriggerEnter(Collider other)
    {   
        Sliced sliced = other.gameObject.GetComponent<Sliced>();
        if (sliced != null)
        {
            sliced.Explode();
        }
    }
}
