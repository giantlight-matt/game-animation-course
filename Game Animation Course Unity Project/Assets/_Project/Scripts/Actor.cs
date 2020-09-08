using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Actor : MonoBehaviour
{

    public  UnityAction OnDie;

    public void Die(){
        if(OnDie != null){
            OnDie.Invoke();
        }
    }
}
