using UnityEngine;

namespace CW_Devyatov_238 {

    [RequireComponent(typeof(Animator))]
    public class BehaviourPlayAnimOnHit : MonoBehaviour {

        public string hitAnimation = "Hit";

        void OnEnable() {
		    UnitActions.onUnitDealDamage += OnHitObject;
	    }

	    void OnDisable() {
		    UnitActions.onUnitDealDamage -= OnHitObject;
	    }

 
        void OnHitObject(GameObject obj, AttackData attackData){
            if(obj == this.gameObject){

                Animator animator = GetComponent<Animator>();
                if(animator){
                    animator.Play(hitAnimation, 0, 0f); //play animation from start

                } else {
                    Debug.Log("Animator component could not be found on " + gameObject.name);
                }

            }
        }
    }
}
