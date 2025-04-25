using UnityEngine;
using System.Collections.Generic;

public enum UNITTYPE
{
    PLAYER = 0,
    ENEMY = 10
}

[System.Serializable]
public class UnitSettings : MonoBehaviour
{
    public UNITTYPE unitType = UNITTYPE.PLAYER;

    public GameObject shadowPrefab;
    public GameObject shadow;
    public GameObject hitEffect;
    public SpriteRenderer hitBox;
    public SpriteRenderer spriteRenderer;

    public DIRECTION startDirection = DIRECTION.RIGHT;
    public float moveSpeed = 4;
    public float moveSpeedAir = 4;
    public bool useAcceleration = false;

    public float moveAcceleration = 25f;
    public float moveDeceleration = 10f;

    public float jumpHeight = 4;
    public float jumpSpeed = 3.5f;
    public float jumpGravity = 5f;
    public float airControlVerticalSpeed = 2f;
    public float groundAdjustmentFactor = 1.75f;

    [Space(10)]
    public List<Combo> comboData = new List<Combo>();

    public float comboResetTime = .55f;
    public bool continueComboOnHit;
    [Space(10)] public AttackData jumpPunch;
    public AttackData jumpKick;
    [Space(10)] public AttackData grabPunch;
    public AttackData grabKick;
    public AttackData grabThrow;
    [Space(10)] public AttackData groundPunch;
    public AttackData groundKick;

    public List<AttackData> enemyAttackList = new List<AttackData>();

    public bool canBeKnockedDown = true;
    public float knockDownHeight = 3;
    public float knockDownDistance = 3;
    public float knockDownSpeed = 3;
    public float knockDownFloorTime = 1;
    public bool hitOtherEnemiesWhenFalling = false;

    public float throwHeight = 3;
    public float throwDistance = 5;
    public bool hitOtherEnemiesWhenThrown = true;

    public float defendChance;
    public float defendDuration;
    public bool canChangeDirWhileDefending;
    public bool rearDefenseEnabled;

    public bool canBeGrabbed = true;
    public string grabAnimation = "Grab";
    public Vector2 grabPosition = new Vector2(0.93f, 0);
    public float grabDuration = 3f;

    public TextAsset unitNamesList;

    public float enemyPauseBeforeAttack = .3f;

    public bool enableFOV;
    public float viewDistance = 5f;
    public float viewAngle = 45f;
    public Vector2 viewPosOffset;
    public bool showFOVCone;
    [HideInInspector] public bool targetInSight;
    private UnitActions unitActions => GetComponent<UnitActions>();

    void Start()
    {
        if (!shadow && shadowPrefab) shadow = GameObject.Instantiate(shadowPrefab, transform.parent) as GameObject;
        if (hitBox) hitBox.color = Color.clear;
        else Debug.LogError(
            "Please assign a HitBox to GameObject " + gameObject.name +
            " in UnitSettings/Linked Components");
        if (spriteRenderer == null)
            Debug.Log(
                "Please assign a SpriteRenderer to GameObject " + gameObject.name +
                " in UnitSettings/Linked Components");
    }

    void Update()
    {
        if (hitBox && hitBox.gameObject.activeSelf)
            MathUtilities.DrawRectGizmo(hitBox.bounds.center, hitBox.bounds.size, Color.red, Time.deltaTime);

        if (shadow)
        {
            shadow.transform.position = new Vector3(transform.position.x, unitActions.groundPos, 0);
            if (spriteRenderer)
                shadow.GetComponent<SpriteRenderer>().sortingOrder =
                    spriteRenderer.sortingOrder - 1;
        }

        ObjectSorting.Sort(spriteRenderer,
            new Vector2(transform.position.x, unitActions ? unitActions.groundPos : transform.position.y));

        targetInSight = unitActions != null ? unitActions.targetInSight() : false;
    }

    string GetRandomName()
    {
        if (unitNamesList == null)
        {
            Debug.Log(
                "no list of unit names was found, please create a .txt file with names on each line, and link it in the unitSettings component.");
            return "";
        }

        string data = unitNamesList.ToString();
        string cReturns = System.Environment.NewLine + "\n" + "\r";
        string[] lines = data.Split(cReturns.ToCharArray());

        string name = "";
        int cnt = 0;
        while (name.Length == 0 && cnt < 100)
        {
            int rand = Random.Range(0, lines.Length);
            name = lines[rand];
            cnt += 1;
        }

        return name;
    }

    private void OnValidate()
    {
        bool toLeft = startDirection == DIRECTION.LEFT;

        transform.localRotation = toLeft
            ? Quaternion.Euler(0f, 180f, 0f)
            : Quaternion.identity;

        foreach (Transform child in transform)
        {
            if (child.CompareTag("HealthBar"))
            {
                child.localRotation = toLeft
                    ? Quaternion.Euler(0f, 180f, 0f)
                    : Quaternion.identity;

                break;
            }
        }
    }
}
