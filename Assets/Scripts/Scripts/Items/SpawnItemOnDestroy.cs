using UnityEngine;

namespace CW_Devyatov_238 {

    //class for spawning items when a GameObject is destroyed
    public class SpawnItemOnDestroy : MonoBehaviour {

        public GameObject itemToSpawn;

        void OnDestroy(){
            if(gameObject.scene.isLoaded){ //gameObject.scene.isLoaded is used to differentiate between a Unity Editor OnDestroy or Runtime OnDestroy

                //spawn item
                if(itemToSpawn) GameObject.Instantiate(itemToSpawn, transform.position, Quaternion.identity);
            }
        }
    }
}
