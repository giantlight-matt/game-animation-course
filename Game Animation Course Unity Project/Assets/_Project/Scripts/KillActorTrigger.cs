using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class KillActorTrigger : MonoBehaviour
{
    public LayerMask AffectedLayers;

    public virtual void OnTriggerEnter2D(Collider2D other){
        var layer = other.gameObject.layer;
        var isAffected = AffectedLayers == (AffectedLayers | (1 << layer));

        if(isAffected){
            Actor actor = other.GetComponent<Actor>();
            if(actor){
                actor.Die();
            }
        }
    }
}
