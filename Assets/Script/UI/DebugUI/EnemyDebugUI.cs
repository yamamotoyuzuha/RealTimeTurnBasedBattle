using TMPro;
using UnityEngine;

public class EnemyDebugUI : MonoBehaviour
{
    [Header("Enemyの攻撃間隔Text")]
    [SerializeField] private TextMeshProUGUI _enemyAttackText;

    private bool isCount;
    private float time;

    void Update()
    {
        if (isCount)
        {
            if (time <= 0)
            {
                isCount = false;
                time = 0;
                return;
            }
            time -= Time.deltaTime;
            UpdateDebugUI();
        }
    }

    public void SetDebugText(float coutTime)
    {
        isCount = true;
        time = coutTime;
        _enemyAttackText.text = time.ToString();
    }

    private void UpdateDebugUI()
    {
        var remainingTime = Mathf.CeilToInt(time);
        _enemyAttackText.text = remainingTime.ToString();
    }
}
