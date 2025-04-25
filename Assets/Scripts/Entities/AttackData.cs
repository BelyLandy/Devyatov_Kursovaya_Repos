using UnityEngine;
using System.Collections.Generic;

    public enum ATTACKTYPE { NONE, PUNCH, GROUNDPOUND, GRAB, GRABPUNCH, GRABTHROW };

    [System.Serializable]
    public class AttackData {
        public string name;
        public int damage;
        public string animationState = "";
        public string sfx = "";
        public ATTACKTYPE attackType = ATTACKTYPE.PUNCH;
        public bool knockdown; //if this attack causes a knockDown or not
        [HideInInspector] public bool foldout;
        [HideInInspector] public GameObject inflictor;
    
        public AttackData(string name, int damage, GameObject inflictor, ATTACKTYPE attackType, bool knockdown, string sfx = ""){
            this.name = name;
            this.damage = damage;
            this.inflictor = inflictor;
            this.attackType = attackType;
            this.knockdown = knockdown;
            this.sfx = sfx;
        }
    }

    [System.Serializable]
    public class Combo {
        public string comboName = "[New Combo]";
        public List<AttackData> attackSequence = new List<AttackData>();
        [HideInInspector] public bool foldout;
    }
