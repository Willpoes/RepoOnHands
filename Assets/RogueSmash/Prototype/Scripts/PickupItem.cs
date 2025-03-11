using System;
using UnityEngine;

namespace MyCompany.RogueSmash.Prototype
{
    [RequireComponent(typeof(BoxCollider))]
    public class PickupItem : MonoBehaviour
    {
        private Action<GameObject> onPickedUp;

        public void Init(Action<GameObject> onPickedUp)
        {
            this.onPickedUp = onPickedUp;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                Debug.Log($"{this.gameObject.name} collected by {other.gameObject.name}");
                onPickedUp?.Invoke(other.gameObject);
                Destroy(gameObject);
            }
        }
    }
}
