using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController2 : MonoBehaviour
{
    float moveSpeed = 5.0f;
    float rotateSpeed = 30.0f;

    public int ID = 1;   // 1P = 1, 2P = 2

    KeyCode forwardKey;
    KeyCode backwardKey;
    KeyCode rightKey;
    KeyCode leftKey;

    [Header("HP Settings")]
    [SerializeField] float maxHp = 100f;
    float currentHp;

    public float CurrentHp => currentHp;
    public float MaxHp => maxHp;

    [Header("Run Settings")]
    [SerializeField] float runSpeed = 10.0f;
    [SerializeField] float hpDecreaseRate = 20.0f;
    KeyCode runKey;

    [Header("Attack Settings")]
    [SerializeField] float attackRange = 1.5f;
    [SerializeField] float knockbackForce = 12.0f;
    [SerializeField] float knockbackDuration = 0.25f;
    KeyCode attackKey;

    private Vector3 knockbackVel;
    private float knockbackTimer;

    [Header("UI Settings")]
    public UnityEngine.UI.Slider hpSlider;
    public UnityEngine.UI.Text hpText;
    public TMPro.TMP_Text hpTextMeshPro;

    [Header("AI Settings")]
    public bool isAI = false;

    private Transform targetGoal;
    private float aiAttackCooldown = 1.5f;
    private float aiAttackTimer = 0f;

    // Start is called before the first frame update
    void Start()
    {
        currentHp = maxHp;

        if(ID == 1)
        {
            forwardKey = KeyCode.W;
            backwardKey = KeyCode.S;
            rightKey = KeyCode.D;
            leftKey = KeyCode.A;
            runKey = KeyCode.LeftShift;
            attackKey = KeyCode.Space;
        }
        else if (ID == 2)
        {
            forwardKey = KeyCode.UpArrow;
            backwardKey = KeyCode.DownArrow;
            rightKey = KeyCode.RightArrow;
            leftKey = KeyCode.LeftArrow;
            runKey = KeyCode.RightShift;
            attackKey = KeyCode.Return;

            // isAI はインスペクターで設定した値をそのまま使用する。
            // タイトル画面のモード選択で上書きしないため、車ごとにAIかどうかを決められる。
        }

        UpdateHpUI();
    }
    void Update()
    { 
        if (knockbackTimer > 0)
        {
            knockbackTimer -= Time.deltaTime;
            transform.Translate(knockbackVel * Time.deltaTime, Space.World);

            UpdateHpUI();
            return;
        }

        if (isAI)
        {
            UpdateAI();
            return;
        }

        float currentSpeed = moveSpeed;

        bool isMoving = Input.GetKey(forwardKey) != Input.GetKey(backwardKey);
        if (Input.GetKey(runKey) && isMoving && currentHp > 0)
        {
            currentSpeed = runSpeed;
            currentHp -= hpDecreaseRate * Time.deltaTime;
            if (currentHp < 0)
            {
                currentHp = 0;
            }
        }

        if(Input.GetKey(forwardKey))
        {
            transform.Translate(0, 0, currentSpeed * Time.deltaTime);
        }

        if(Input.GetKey(backwardKey))
        {
            transform.Translate(0, 0, -currentSpeed * Time.deltaTime);
        }

        if(Input.GetKey(rightKey))
        {

            transform.Rotate(0, rotateSpeed * Time.deltaTime, 0);
        }

        if(Input.GetKey(leftKey))
        {

            transform.Rotate(0, -rotateSpeed * Time.deltaTime, 0);
        }

        UpdateHpUI();

        if (Input.GetKeyDown(attackKey))
        {
            Attack();
        }
        
    }

    void Attack()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position + transform.forward * 1.0f, attackRange);
        foreach (var hitCollider in hitColliders)
        {
            PlayerController2 target = hitCollider.GetComponent<PlayerController2>();
            if (target != null && target != this)
            {
                Vector3 direction = (target.transform.position - transform.position).normalized;
                direction.y = 0;
                
                target.ApplyKnockback(direction * knockbackForce, knockbackDuration);
            }
        }
    }

    public void ApplyKnockback(Vector3 velocity, float duration)
    {
        knockbackVel = velocity;
        knockbackTimer = duration;
    }

    void UpdateAI()
    {
        if (targetGoal == null)
        {
            var goalObj = FindObjectOfType<ManagerOfGoal>();
            if (goalObj != null)
            {
                targetGoal = goalObj.transform;
            }
            else
            {
                var altGoalObj = FindObjectOfType<GoalManager>();
                if (altGoalObj != null)
                {
                    targetGoal = altGoalObj.transform;
                }
                else
                {
                    var namedGoal = GameObject.Find("Goal");
                    if (namedGoal != null)
                    {
                        targetGoal = namedGoal.transform;
                    }
                }
            }
        }

        float currentSpeed = moveSpeed;
        if (targetGoal != null)
        {
            Vector3 direction = (targetGoal.position - transform.position);
            direction.y = 0;
            
            if (direction.magnitude > 0.5f)
            {
                float angle = Vector3.SignedAngle(transform.forward, direction, Vector3.up);

                if (angle > 2f)
                {
                    transform.Rotate(0, rotateSpeed * Time.deltaTime, 0);
                }
                else if (angle < -2f)
                {
                    transform.Rotate(0, -rotateSpeed * Time.deltaTime, 0);
                }

                bool shouldRun = currentHp > 0 && Mathf.Abs(angle) < 30f;
                if (shouldRun)
                {
                    currentSpeed = runSpeed;
                    currentHp -= hpDecreaseRate * Time.deltaTime;
                    if (currentHp < 0) currentHp = 0;
                }

                transform.Translate(0, 0, currentSpeed * Time.deltaTime);
            }
        }

        UpdateHpUI();

        if (aiAttackTimer > 0)
        {
            aiAttackTimer -= Time.deltaTime;
        }

        if (aiAttackTimer <= 0)
        {
            PlayerController2 opponent = GetOpponent();
            if (opponent != null)
            {
                float dist = Vector3.Distance(transform.position, opponent.transform.position);
                if (dist <= attackRange * 1.1f)
                {
                    Attack();
                    aiAttackTimer = aiAttackCooldown;
                }
            }
        }
    }

    PlayerController2 GetOpponent()
    {
        PlayerController2[] players = FindObjectsOfType<PlayerController2>();
        foreach (var p in players)
        {
            if (p.ID != this.ID)
            {
                return p;
            }
        }
        return null;
    }

    private void UpdateHpUI()
    {
        if (hpSlider != null)
        {
            hpSlider.maxValue = maxHp;
            hpSlider.value = currentHp;
        }

        if (hpText != null)
        {
            hpText.text = string.Format("HP: {0:0} / {1:0}", currentHp, maxHp);
        }

        if (hpTextMeshPro != null)
        {
            hpTextMeshPro.text = string.Format("HP: {0:0} / {1:0}", currentHp, maxHp);
        }
    }
}
