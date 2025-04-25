using UnityEngine;


    public class StateMachine : UnitActions {
    
        [SerializeField] private bool showStateInGame;
        [HideInInspector] public string currentState;
        private TextMesh stateText;
        private State state;

        void Start(){

            if(isPlayer) SetState(new PlayerIdle());
            else if(isEnemy) SetState(new EnemyIdle());
        }

        public void SetState(State _state){
        
            //exit current state
            if (this.state != null) state.Exit();
       
            //set new state
            state = _state;
            state.unit = this;

            //set data
            currentState = GetCurrentStateShortName();
            state.stateStartTime = Time.time;

            //enter the state
            state.Enter();
        }

        public State GetCurrentState(){
            return state;;
        }

        void Update(){
            state?.Update();
            UpdateStateText();
        }

        void LateUpdate(){
            state?.LateUpdate();
        }

        void FixedUpdate(){
            state?.FixedUpdate();
        }

        void UpdateStateText(){

            if(!showStateInGame){
                if (stateText != null) {
                    Destroy(stateText.gameObject);
                    stateText = null;
                }
                return;
            }

            if(stateText == null){
                GameObject stateTxtGo = Instantiate(Resources.Load("StateText"), transform) as GameObject;
                if (stateTxtGo != null) {
                    stateTxtGo.name = "StateText";
                    stateTxtGo.transform.localPosition = new Vector2(0, -0.2f);
                    stateText = stateTxtGo.GetComponent<TextMesh>();
                }
            }

            if(stateText != null){
                stateText.text = GetCurrentStateShortName();
                stateText.transform.localRotation = Quaternion.Euler(0, dir == DIRECTION.LEFT ? 180 : 0, 0);
            }
        }

        string GetCurrentStateShortName(){
            string currentState = stateMachine?.GetCurrentState().GetType().ToString();
            string[] splitStrings = currentState.Split('.');                  
            if(splitStrings.Length >= 2) return splitStrings[1];
            return "";
        }
    }
