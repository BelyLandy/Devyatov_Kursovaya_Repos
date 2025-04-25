using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Reflection;

namespace CW_Devyatov_238 {
    [CanEditMultipleObjects]
    [CustomEditor(typeof(UnitSettings))]
    class UnitSettingsEditor : Editor {
        public static Dictionary<string, bool> foldOutList = new Dictionary<string, bool> {
            { "linkedComponentsFoldout", false },
            { "movementFoldout", false },
            { "jumpFoldout", false },
            { "attackDataFoldout", false },
            { "comboDataFoldout", false },
            { "knockDownFoldout", false },
            { "throwFoldout", false },
            { "defenceFoldout", false },
            { "grabFoldout", false },
            { "weaponFoldout", false },
            { "unitNameFoldout", false },
            { "fovFoldout", false },
        };

        private SerializedProperty[] properties;
        private HashSet<string> linkedComponentFields = new HashSet<string>{ "shadowPrefab", "weaponBone", "hitEffect", "hitBox", "spriteRenderer" };
        private HashSet<string> movementFields = new HashSet<string>{ "startDirection", "moveSpeed", "moveSpeedAir", "useAcceleration" };
        private HashSet<string> accelerationFields = new HashSet<string>{ "moveAcceleration", "moveDeceleration" };
        private HashSet<string> jumpFields = new HashSet<string>{ "jumpHeight", "jumpSpeed", "jumpGravity" };
        private HashSet<string> attackDataFields = new HashSet<string> { "jumpPunch", "jumpKick", "grabPunch", "grabKick", "grabThrow", "groundPunch", "groundKick" };
        private HashSet<string> comboDataFields = new HashSet<string> { "comboResetTime", "continueComboOnHit" };
        private HashSet<string> knockdownFields = new HashSet<string> { "knockDownHeight", "knockDownDistance", "knockDownSpeed", "knockDownFloorTime", "hitOtherEnemiesDuringFall", "hitOtherEnemiesWhenFalling" };
        private HashSet<string> throwFields = new HashSet<string> { "throwHeight", "throwDistance", "hitOtherEnemiesWhenThrown" };
        private HashSet<string> defenceFieldsPlayer = new HashSet<string> { "canChangeDirWhileDefending", "rearDefenseEnabled" };
        private HashSet<string> defenceFieldsEnemy = new HashSet<string> { "defendChance", "defendDuration", "rearDefenseEnabled" };
        private HashSet<string> grabFields = new HashSet<string> { "grabAnimation", "grabPosition", "grabDuration" };
        private HashSet<string> weaponFields = new HashSet<string> { "loseWeaponWhenHit", "loseWeaponWhenKnockedDown" };
        private HashSet<string> unitNameFieldsPlayer = new HashSet<string> { "unitName", "unitPortrait", "showNameInAllCaps" };
        private HashSet<string> unitNameFieldsEnemy = new HashSet<string> { "unitName", "showNameInAllCaps", "unitPortrait", "loadRandomNameFromList" };
        private HashSet<string> fovFields = new HashSet<string> { "enableFOV", "viewDistance", "viewAngle", "viewPosOffset", "showFOVCone", "targetInSight" };
        
        private Texture2D iconArrowClose;
        private Texture2D iconArrowOpen;
        private Texture2D iconInfo;

        private DIRECTION prevDirection = DIRECTION.LEFT;
        private string space = "  ";

        void OnEnable() {
            iconArrowClose = Resources.Load<Texture2D>("IconArrowClose");
            iconArrowOpen = Resources.Load<Texture2D>("IconArrowOpen");
            iconInfo = Resources.Load<Texture2D>("IconInfo");
            CacheSerializedProperties();
        }

        public override void OnInspectorGUI() {
            var settings = (UnitSettings)target;
            if (settings == null) return;

            Undo.RecordObject(settings, "Undo change settings");

            serializedObject.Update();
            EditorGUI.BeginChangeCheck();

            MainContent(settings);
            
            if (EditorGUI.EndChangeCheck()) {
                serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(settings);
            }
        }

        void MainContent(UnitSettings settings) {
            DrawPropertyField("unitType");

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(new GUIContent(space + "Linked Components", GetArrow(foldOutList["linkedComponentsFoldout"])), FoldOutStyle(), GUILayout.ExpandWidth(true), GUILayout.Height(30)))
                foldOutList["linkedComponentsFoldout"] = !foldOutList["linkedComponentsFoldout"];
            EditorGUILayout.EndHorizontal();

            if (foldOutList["linkedComponentsFoldout"]) {
                EditorGUI.indentLevel++;
                DrawPropertyFields(linkedComponentFields);
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(new GUIContent(space + "Movement Settings", GetArrow(foldOutList["movementFoldout"])), FoldOutStyle(), GUILayout.ExpandWidth(true), GUILayout.Height(30)))
                foldOutList["movementFoldout"] = !foldOutList["movementFoldout"];
            EditorGUILayout.EndHorizontal();

            if (foldOutList["movementFoldout"]) {
                EditorGUI.indentLevel++;
                DrawPropertyFields(movementFields);

                if (prevDirection != settings.startDirection) {
                    settings.transform.localRotation = (settings.startDirection == DIRECTION.LEFT) ? Quaternion.Euler(0, 180, 0) : Quaternion.identity;
                    prevDirection = settings.startDirection;
                }

                if (settings.useAcceleration) {
                    EditorGUI.indentLevel++;
                    DrawPropertyFields(accelerationFields);
                    EditorGUI.indentLevel--;
                }
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(new GUIContent(space + "Jump Settings", GetArrow(foldOutList["jumpFoldout"])), FoldOutStyle(), GUILayout.ExpandWidth(true), GUILayout.Height(30)))
                foldOutList["jumpFoldout"] = !foldOutList["jumpFoldout"];
            EditorGUILayout.EndHorizontal();

            if (foldOutList["jumpFoldout"]) {
                EditorGUI.indentLevel++;
                DrawPropertyFields(jumpFields);
                EditorGUI.indentLevel--;
            }

            if (settings.unitType == UNITTYPE.PLAYER) {
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button(new GUIContent(space + "Attack Data", GetArrow(foldOutList["attackDataFoldout"])), FoldOutStyle(), GUILayout.ExpandWidth(true), GUILayout.Height(30)))
                    foldOutList["attackDataFoldout"] = !foldOutList["attackDataFoldout"];
                EditorGUILayout.EndHorizontal();

                if (foldOutList["attackDataFoldout"]) {
                    EditorGUI.indentLevel++;
                    foreach (string attack in attackDataFields)
                        ShowAttackData(GetPropertyByName(attack), false);
                    EditorGUI.indentLevel--;
                }

                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button(new GUIContent(space + "Combo Data", GetArrow(foldOutList["comboDataFoldout"])), FoldOutStyle(), GUILayout.ExpandWidth(true), GUILayout.Height(30)))
                    foldOutList["comboDataFoldout"] = !foldOutList["comboDataFoldout"];
                EditorGUILayout.EndHorizontal();

                if (foldOutList["comboDataFoldout"]) {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.Space(5);
                    ShowHeader("Combo Settings");
                    DrawPropertyFields(comboDataFields);
                    EditorGUILayout.Space(5);
                    ShowHeader("Combo List");
                    ShowComboData(settings.comboData);
                    EditorGUI.indentLevel--;
                }
            }

            if (settings.unitType == UNITTYPE.ENEMY) {
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button(new GUIContent(space + "Attack Data", GetArrow(foldOutList["attackDataFoldout"])), FoldOutStyle(), GUILayout.ExpandWidth(true), GUILayout.Height(30)))
                    foldOutList["attackDataFoldout"] = !foldOutList["attackDataFoldout"];
                EditorGUILayout.EndHorizontal();

                if (foldOutList["attackDataFoldout"]) {
                    EditorGUI.indentLevel++;
                    if (settings.unitType == UNITTYPE.ENEMY)
                        DrawPropertyField("enemyPauseBeforeAttack");
                    EditorGUILayout.Space(5);
                    ShowHeader("Enemy Attack List");
                    ShowEnemyAttackData();                  
                    EditorGUI.indentLevel--;
                }
            }

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(new GUIContent(space + "KnockDown Settings", GetArrow(foldOutList["knockDownFoldout"])), FoldOutStyle(), GUILayout.ExpandWidth(true), GUILayout.Height(30)))
                foldOutList["knockDownFoldout"] = !foldOutList["knockDownFoldout"];
            EditorGUILayout.EndHorizontal();

            if (foldOutList["knockDownFoldout"]) {
                EditorGUI.indentLevel++;
                DrawPropertyField("canBeKnockedDown");
                if (settings.canBeKnockedDown)
                    DrawPropertyFields(knockdownFields);
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(new GUIContent(space + "Throw Settings", GetArrow(foldOutList["throwFoldout"])), FoldOutStyle(), GUILayout.ExpandWidth(true), GUILayout.Height(30)))
                foldOutList["throwFoldout"] = !foldOutList["throwFoldout"];
            EditorGUILayout.EndHorizontal();
            
            if (foldOutList["throwFoldout"]) {
                EditorGUI.indentLevel++;
                DrawPropertyFields(throwFields);
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(new GUIContent(space + "Defence Settings", GetArrow(foldOutList["defenceFoldout"])), FoldOutStyle(), GUILayout.ExpandWidth(true), GUILayout.Height(30)))
                foldOutList["defenceFoldout"] = !foldOutList["defenceFoldout"];
            EditorGUILayout.EndHorizontal();

            if (foldOutList["defenceFoldout"]) {
                EditorGUI.indentLevel++;
                if (settings.unitType == UNITTYPE.ENEMY)
                    DrawPropertyFields(defenceFieldsEnemy);
                else if (settings.unitType == UNITTYPE.PLAYER)
                    DrawPropertyFields(defenceFieldsPlayer);
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(new GUIContent(space + "Grab Settings", GetArrow(foldOutList["grabFoldout"])), FoldOutStyle(), GUILayout.ExpandWidth(true), GUILayout.Height(30)))
                foldOutList["grabFoldout"] = !foldOutList["grabFoldout"];
            EditorGUILayout.EndHorizontal();

            if (foldOutList["grabFoldout"]) {
                EditorGUI.indentLevel++;
                if (settings.unitType == UNITTYPE.PLAYER)
                    DrawPropertyFields(grabFields);
                if (settings.unitType == UNITTYPE.ENEMY)
                    DrawPropertyField("canBeGrabbed");
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(new GUIContent(space + "Unit Name & Portrait", GetArrow(foldOutList["unitNameFoldout"])), FoldOutStyle(), GUILayout.ExpandWidth(true), GUILayout.Height(30)))
                foldOutList["unitNameFoldout"] = !foldOutList["unitNameFoldout"];
            EditorGUILayout.EndHorizontal();

            if (foldOutList["unitNameFoldout"]) {
                EditorGUI.indentLevel++;
                if (settings.unitType == UNITTYPE.PLAYER)
                    DrawPropertyFields(unitNameFieldsPlayer);
                if (settings.unitType == UNITTYPE.ENEMY) {
                    DrawPropertyFields(unitNameFieldsEnemy);
                }
                EditorGUI.indentLevel--;
            }

            if (settings.unitType == UNITTYPE.ENEMY) {
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button(new GUIContent(space + "Field Of View Settings", GetArrow(foldOutList["fovFoldout"])), FoldOutStyle(), GUILayout.ExpandWidth(true), GUILayout.Height(30)))
                    foldOutList["fovFoldout"] = !foldOutList["fovFoldout"];
                EditorGUILayout.EndHorizontal();
                if (foldOutList["fovFoldout"]) {
                    EditorGUI.indentLevel++;
                    DrawPropertyFields(fovFields);
                    EditorGUI.indentLevel--;
                }
            }
        }

        void ShowEnemyAttackData() {
            SerializedProperty enemyAttackListProperty = serializedObject.FindProperty("enemyAttackList");
            if (enemyAttackListProperty != null && enemyAttackListProperty.isArray) {
                if (enemyAttackListProperty.arraySize == 0)
                    EditorGUILayout.LabelField("No Enemy Attack Data Available");
                for (int i = 0; i < enemyAttackListProperty.arraySize; i++) {
                    SerializedProperty attackDataProperty = enemyAttackListProperty.GetArrayElementAtIndex(i);
                    ShowAttackData(attackDataProperty, true);
                }
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(" ", GUILayout.Width(17));
                if (enemyAttackListProperty.arraySize > 0) {
                    if (GUILayout.Button("-", smallButtonStyle()))
                        enemyAttackListProperty.DeleteArrayElementAtIndex(enemyAttackListProperty.arraySize - 1);
                }
                if (GUILayout.Button("+", smallButtonStyle(), GUILayout.Width(25)))
                    enemyAttackListProperty.InsertArrayElementAtIndex(enemyAttackListProperty.arraySize);
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Space(10);
            }
        }

        void ShowComboData(List<Combo> comboList) {
            if (comboList.Count == 0)
                EditorGUILayout.LabelField("No Combo Data Available");
            foreach (Combo combo in comboList) {
                combo.foldout = EditorGUILayout.Foldout(combo.foldout, combo.comboName, true);
                if (combo.foldout) {
                    combo.comboName = EditorGUILayout.TextField("Combo Name:", combo.comboName);
                    ShowHeader("Attack Sequence");
                    if (combo.attackSequence.Count == 0)
                        EditorGUILayout.LabelField("This combo does not have any attacks listed");
                    foreach (AttackData data in combo.attackSequence) {
                        EditorGUI.indentLevel++;
                        data.foldout = EditorGUILayout.Foldout(data.foldout, data.name, true);
                        if (data.foldout) {
                            data.name = EditorGUILayout.TextField("Attack Name:", data.name);
                            data.damage = EditorGUILayout.IntField("Damage", data.damage);
                            data.sfx = EditorGUILayout.TextField("Sfx (on hit)", data.sfx);
                            data.animationState = EditorGUILayout.TextField(N/AVenice truncated due to length
