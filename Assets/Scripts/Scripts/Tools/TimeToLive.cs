using UnityEngine;
using System.Collections;

namespace CW_Devyatov_238 {

    //class for destroying objects after x seconds
    public class TimeToLive : MonoBehaviour {
    
        public float timeToLive = 1f;

        IEnumerator Start(){
            yield return new WaitForSeconds(timeToLive);
            Destroy(gameObject);
        }
    }
}
