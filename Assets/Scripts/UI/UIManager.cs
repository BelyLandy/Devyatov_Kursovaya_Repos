using UnityEngine;

    public class UIManager : MonoBehaviour {

        public UIMenu[] menuList;

        public void ShowMenu(string menuName){
            
            UIMenu menuToOpen = null;
            foreach(UIMenu menu in menuList) if(menu.menuName == menuName) menuToOpen = menu;
            if(menuToOpen == null) { Debug.Log("'" + menuName + "' could not be found in UIManager"); return; } //show error message if this menu does not exist
            
            foreach(UIMenu menu in menuList) menu.menuGameObject.SetActive(false);
            
            menuToOpen.menuGameObject.SetActive(true);
        }
    }

    [System.Serializable] 
    public class UIMenu {
        public string menuName;
        public GameObject menuGameObject;
    }
