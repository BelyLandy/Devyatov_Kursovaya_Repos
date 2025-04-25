using UnityEngine;

namespace CW_Devyatov_238 {
    
    public class SpawnItemOnDestroy : MonoBehaviour {

        public GameObject itemToSpawn;

        void OnDestroy(){
            if(gameObject.scene.isLoaded){
                
                if(itemToSpawn) GameObject.Instantiate(itemToSpawn, transform.position, Quaternion.identity);
            }
        }
    }
}
