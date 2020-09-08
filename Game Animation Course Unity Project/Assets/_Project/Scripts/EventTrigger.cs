using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventTrigger : MonoBehaviour
{
    public UnityEvent OnEnter;
    public UnityEvent OnExit;
    public LayerMask AffectedLayers;


    public virtual void OnTriggerEnter2D(Collider2D other){
        var layer = other.gameObject.layer;
        var isAffected = AffectedLayers == (AffectedLayers | (1 << layer));

        if(isAffected){
            if(OnEnter != null){
                OnEnter.Invoke();
            }
        }
    }

    public virtual void OnTriggerExit2D(Collider2D other){
        var layer = other.gameObject.layer;
        var isAffected = AffectedLayers == (AffectedLayers | (1 << layer));
        if(isAffected){
            if(OnExit != null){
                OnExit.Invoke();
            }
        }
    }
}
