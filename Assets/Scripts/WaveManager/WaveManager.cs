using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;


public class WaveManager : MonoBehaviour {
    
        [Header("Wave Data")]
        private int totalNumberOfWaves;
        private int currentWave;
        private int enemiesLeftInThisWave = 0;
        private  int totalEnemiesLeft = 0;
        public bool endLevelWhenAllEnemiesAreDefeated = true;
        public bool triggerSlowMotionOnLastEnemyKill = true;
        private bool slowMotionInProgress = false;
        private bool smoothLevelBoundTransition = true;
        private float levelBoundTransitionDuration = 2f;
        private List<GameObject> enemyWaves = new List<GameObject>();

        [Header("Menu List")]
        [SerializeField] private string menuToOpenOnLevelFinish = "LevelCompleted";
        [SerializeField] private string menuToOpenOnPlayerDeath = "GameOver";

        void OnEnable() {
		    HealthController.OnUnitDeath += OnUnitDeath;
	    }

	    void OnDisable() {
		    HealthController.OnUnitDeath -= OnUnitDeath;
	    }

        void Start(){
            GetAllEnemyWaves();
            DisableAllWaves();
            ActivateWave(0);
            totalEnemiesLeft = EnemyManager.GetTotalEnemyCount();
        }
        
        void GetAllEnemyWaves(){
            for(int i=0; i<transform.childCount; i++) enemyWaves.Add(transform.GetChild(i).gameObject);
            totalNumberOfWaves = enemyWaves.Count;
        }
        
        void DisableAllWaves(){
             foreach(GameObject wave in enemyWaves) wave.SetActive(false);
        }
        
        void ActivateWave(int wave){
            
            if(enemyWaves.Count == 0 && wave == 0) return;
            
            if(wave >= enemyWaves.Count || EnemyManager.GetTotalEnemyCount() == 0){
                OnFinish(); 
                return; 
            }
            
            currentWave = wave;
            if(enemyWaves[wave] == null) return;
            else enemyWaves[wave].SetActive(true);
            
            if(smoothLevelBoundTransition && currentWave>0) StartCoroutine(LevelBoundTransition());
            else SetCameraLevelBound();
            
            enemiesLeftInThisWave = EnemyManager.GetCurrentEnemyCount();
        }
        
        public void OnUnitDeath(GameObject unit){
            
            totalEnemiesLeft = EnemyManager.GetTotalEnemyCount();
            
            if(unit.GetComponent<UnitSettings>()?.unitType == UNITTYPE.PLAYER){
                OnPlayerDeath();
                return;
            }
            
            if(unit.GetComponent<UnitSettings>()?.unitType == UNITTYPE.ENEMY){
                
                if(triggerSlowMotionOnLastEnemyKill && totalEnemiesLeft == 0) StartCoroutine(StartSlowMotionEffectRoutine());
                
                enemiesLeftInThisWave = EnemyManager.GetCurrentEnemyCount();
                if(enemiesLeftInThisWave == 0) ActivateWave(currentWave + 1);
           }
        }
        
        IEnumerator LevelBoundTransition(){
            
            LevelBound LevelBound1 = enemyWaves[currentWave-1].GetComponentInChildren<LevelBound>();
            LevelBound LevelBound2 = enemyWaves[currentWave].GetComponentInChildren<LevelBound>();
            
            if(!LevelBound1 || !LevelBound2){ SetCameraLevelBound(); yield break; } 
            
            Vector3 From = LevelBound1.transform.position;
            Vector3 To  = LevelBound2.transform.position;

            float t=0;
            while(t<1){
                LevelBound1.transform.position = Vector3.Lerp(From, To, MathUtilities.CoSinLerp(t));
                t += Time.deltaTime / levelBoundTransitionDuration;
                yield return 0;
            }

            SetCameraLevelBound();
        }
        
        void SetCameraLevelBound(){
            
            LevelBound currentWaveLevelBound = enemyWaves[currentWave].GetComponentInChildren<LevelBound>();
            
            CameraFollow camFol = Camera.main.GetComponent<CameraFollow>();
            if(currentWaveLevelBound != null && camFol != null) camFol.levelBound = currentWaveLevelBound;
            
            if(currentWave>0) Destroy(enemyWaves[currentWave-1].gameObject);
        }
        
        void OnFinish()
        {
            currentWave = totalNumberOfWaves;

            if (!endLevelWhenAllEnemiesAreDefeated) return;

            SaveLevelProgress();
            
            UIManager ui = Object.FindFirstObjectByType<UIManager>(
                FindObjectsInactive.Include);   // include disabled canvases

            if (ui == null)
            {
                Debug.Log("No UI Manager found in this level; can't show level-completed screen");
                return;
            }

            ui.ShowMenu(menuToOpenOnLevelFinish);
        }
        
        void SaveLevelProgress(){
            string currentLevelScene = SceneManager.GetActiveScene().name;
            //if(!LevelProgress.levelsCompleted.Contains(currentLevelScene)) LevelProgress.levelsCompleted.Add(currentLevelScene);
        }
        
        IEnumerator StartSlowMotionEffectRoutine(){
            if(slowMotionInProgress) yield break;
            Time.timeScale = .5f;
            yield return new WaitForSecondsRealtime(1.5f);
            while(Time.timeScale < 1f) Time.timeScale += Time.deltaTime;
            Time.timeScale = 1f;
        }
        
        void OnPlayerDeath()
        {
            UIManager ui = Object.FindFirstObjectByType<UIManager>(
                FindObjectsInactive.Include);

            if (ui) ui.ShowMenu(menuToOpenOnPlayerDeath);
        }
    }
