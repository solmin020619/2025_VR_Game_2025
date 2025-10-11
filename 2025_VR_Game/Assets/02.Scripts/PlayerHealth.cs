using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHP = 100;
    private int currentHP;

    // Start is called before the first frame update
    void Start() => currentHP = maxHP;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Trap"))
        {
            TakeDamage(20);
        }
    }

    void TakeDamage(int amount)
    {
        currentHP -= amount;
        Debug.Log($"피해! 남은 체력: {currentHP}");

        if (currentHP <= 0)
        {
            Debug.Log("사망! 게임오버 처리");
            //Game Manager에 신호 보내기
        }
    }




    // Update is called once per frame
    void Update()
    {
        
    }
}
