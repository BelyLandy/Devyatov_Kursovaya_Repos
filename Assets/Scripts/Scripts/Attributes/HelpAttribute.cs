using UnityEngine;

namespace CW_Devyatov_238 {

    public class HelpAttribute : PropertyAttribute {
        public string text;
        public HelpAttribute(string text) {
            this.text = text;
        }
    }
}