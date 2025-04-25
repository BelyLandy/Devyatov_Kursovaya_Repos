using UnityEngine;

namespace CW_Devyatov_238 {

    public class HealthPickup : Item_Obj {

        [Header("Health Setting")]
        public int healthRecover = 1;
        public GameObject showEffect;

        public override void OnPickUpItem(GameObject target){

            target?.GetComponent<HealthController>()?.AddHealth(healthRecover);

            if(showEffect) GameObject.Instantiate(showEffect, transform.position, Quaternion.identity);

            AudioController.PlaySFX(pickupSFX, transform.position);
            Destroy(gameObject);
        }
    }
}
