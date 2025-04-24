using UnityEngine;

namespace CW_Devyatov_238 {

    //class for Health Pickup Items
    public class HealthPickup : Item {

        [Header("Health Setting")]
        public int healthRecover = 1;
        public GameObject showEffect; //effect to show when picked up

        //item has been picked up
        public override void OnPickUpItem(GameObject target){

            //heal target if this is a health pickup
            target?.GetComponent<HealthController>()?.AddHealth(healthRecover);

            //show effect
            if(showEffect) GameObject.Instantiate(showEffect, transform.position, Quaternion.identity);

            //play sfx
            AudioController.PlaySFX(pickupSFX, transform.position);
            Destroy(gameObject);
        }
    }
}
