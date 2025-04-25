using UnityEngine;

    public class ObjectSorting : MonoBehaviour {

        public static int SortingStep = -50;

        public virtual void Start(){
            Renderer rend = GetComponent<Renderer>();
            if(rend) ObjectSorting.Sort(rend, transform.position);
        }

        public static void Sort(Renderer rend, Vector2 position){
            if(rend) rend.sortingOrder = (int)(position.y * SortingStep);
        }

        private void OnValidate(){
            Sort(GetComponent<Renderer>(), transform.position);
        }
    }
