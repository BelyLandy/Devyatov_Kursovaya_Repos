using UnityEngine;

namespace CW_Devyatov_238 {

    //this class handles the opening of menu's
    public class UIManager : MonoBehaviour {

        public UIMenu[] menuList;

        public void ShowMenu(string menuName){

            //find menu to open
            UIMenu menuToOpen = null;
            foreach(UIMenu menu in menuList) if(menu.menuName == menuName) menuToOpen = menu;
            if(menuToOpen == null) { Debug.Log("'" + menuName + "' could not be found in UIManager"); return; } //show error message if this menu does not exist

            //disable all menu's
            foreach(UIMenu menu in menuList) menu.menuGameObject.SetActive(false);

            //open menu
            menuToOpen.menuGameObject.SetActive(true);
        }
    }

    [System.Serializable] 
    public class UIMenu {
        public string menuName;
        public GameObject menuGameObject;
    }
}