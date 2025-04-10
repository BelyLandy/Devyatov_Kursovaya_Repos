using UnityEngine;
using UnityEngine.UI;

namespace CW_Devyatov_238 {

    //class for unit or objects healthbars in the User Interface
    public class UIHUDHealthBar : MonoBehaviour {

        private enum HEALTHBARTYPE { PlayerHealthBar, EnemyHealthBar, BossHealthBar }
        [SerializeField] private HEALTHBARTYPE healthBarType = HEALTHBARTYPE.PlayerHealthBar;
	    //public Text nameField;
	    //public Image portrait;
	    public Image healthBar;
        private bool initialized;

	    void OnEnable() {
		    HealthController.OnHealthChanged += UpdateHealth; //subscribe to health update events
            if(healthBarType == HEALTHBARTYPE.EnemyHealthBar) ShowHealthBar(false); //hide enemy healthbar by default

            //check portrait reference
		    //if(portrait == null) Debug.Log("no portrait image was linked in the UIHealthbar"); 
	    }

	    void OnDisable() {
		    HealthController.OnHealthChanged -= UpdateHealth; //unsubscribe to health update events
	    }

	    void UpdateHealth(HealthController hs){
            if(healthBar == null) return;

            //update player healthbar
		    if(healthBarType == HEALTHBARTYPE.PlayerHealthBar && hs.IsPlayer){
                if(!initialized) InitializePlayerBar(hs); //this is only done once at the start of the level
                healthBar.fillAmount = hs.HealthPercent;
            }

            //update enemy healthbar
		    if(hs.IsEnemy){

                //check if we need to show the regular enemy healthbar or large boss healthbar
                bool showBossHealthBar = (healthBarType == HEALTHBARTYPE.BossHealthBar && hs.DisplayLargeHealthBar);
                bool showDefaultEnemyHealthBar = (healthBarType == HEALTHBARTYPE.EnemyHealthBar && !hs.DisplayLargeHealthBar);

                if(showDefaultEnemyHealthBar || showBossHealthBar){
                    ShowHealthBar(true);
                    //SetUnitPortrait(hs);
                    healthBar.gameObject.SetActive(true);
			        healthBar.fillAmount = hs.HealthPercent;
			        //nameField.text = hs.GetComponent<UnitSettings>().unitName; //get enemy name from unit settings
                    //if(hs.GetComponent<UnitSettings>().showNameInAllCaps) nameField.text = nameField.text.ToUpper(); //show in capital letters
			        if(hs.CurrentHealth == 0) ShowHealthBar(false); //hide enemy healthbar when hp = 0
                }
		    }
	    }

	    /*//loads the HUD icon of the player from the player prefab (HealthController)
	    void SetUnitPortrait(HealthController hs){
            //if(hs == null || portrait == null) return;

            //load portrait icon from unit settings
            UnitSettings settings = hs.GetComponent<UnitSettings>();
            portrait.sprite = (settings.unitPortrait != null)?  settings.unitPortrait : null;
            portrait.enabled = (portrait.sprite != null); //only show portrait is there is a sprite
	    }*/

        //load player data on initialize
        void InitializePlayerBar(HealthController hs){
            //SetUnitPortrait(hs); //get portrait
            //nameField.text = hs.GetComponent<UnitSettings>().unitName; //get name
            //if(hs.GetComponent<UnitSettings>().showNameInAllCaps) nameField.text = nameField.text.ToUpper(); //show in capital letters
            initialized = true;
        }

        //show or hide this healthbar
        void ShowHealthBar(bool state) {
            CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
            if(canvasGroup != null) canvasGroup.alpha = state? 1f : 0f;
        }
    }
}
