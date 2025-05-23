using UnityEngine;
using System.Collections;

namespace CW_Devyatov_238 {

    public class AnimationCurveAnim : MonoBehaviour {

        [Header("Y Position")]
        public AnimationCurve AnumCurveY;
        public float ScaleY = 1;

        [Header("X Position")]
        public AnimationCurve AnumCurveX;
        public float ScaleX = 1;

        [Header("Settings")]
        public float Duration = 1;

        void  Start(){
            StartCoroutine(PlayBounceAnim());
        }

        IEnumerator PlayBounceAnim(){
            if(AnumCurveY.length == 0) yield break;
            float duration = Mathf.Max(AnumCurveX[AnumCurveX.length-1].time * Duration, AnumCurveY[AnumCurveY.length-1].time * Duration);
            Vector3 startPos = transform.position;
        
            float t=0;
            while(t<duration){
                transform.position = startPos + (Vector3.up * AnumCurveY.Evaluate(t) * ScaleY) + (Vector3.right * AnumCurveX.Evaluate(t) * ScaleX) ;
                t += Time.deltaTime / duration;
                yield return 0;
            }
        }
    }
}