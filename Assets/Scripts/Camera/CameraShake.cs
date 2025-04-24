using UnityEngine;
using System.Collections;

namespace CW_Devyatov_238 {

    [RequireComponent(typeof(CameraFollow))]
    public class CameraShake : MonoBehaviour {

        public AnimationCurve CameraShakeAnimation;
        public float intensity = .15f;
        public float duration = .3f;

        private float yOffset;
        private CameraFollow cf => GetComponent<CameraFollow>();

        public void ShowCamShake(){
             StartCoroutine(camShakeRoutine(intensity, duration));
        }

        public void ShowCamShake(float _intensity, float _duration){
             StartCoroutine(camShakeRoutine(_intensity, _duration));
        }

        IEnumerator camShakeRoutine(float _intensity, float _duration){
            if(CameraShakeAnimation.length == 0) yield break;
            float animCurveDuration = (CameraShakeAnimation[CameraShakeAnimation.length-1].time); 

            float t=0;
            while(t<animCurveDuration){

                yOffset = CameraShakeAnimation.Evaluate(t) * _intensity;

                if(cf != null) cf.additionalYOffset = yOffset;

                t += Time.deltaTime / _duration;
                yield return 0;
            }
            yOffset = 0;
        }
    }
}