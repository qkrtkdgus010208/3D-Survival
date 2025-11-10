using System;
using System.Collections;
using UnityEngine;

public interface IDamagable
{
    bool IsInvulnerable { get; }
    void TakePhysicalDamage(int damageAmount);
}

public class PlayerCondition : MonoBehaviour, IDamagable
{
    public UICondition uiCondition;

    Condition health { get { return uiCondition.health; } }
    Condition hunger { get { return uiCondition.hunger; } }
    Condition stamina { get { return uiCondition.stamina; } }

    public float noHungerHealthDecay;
    public event Action onTakeDamage;

    Coroutine healCoroutine;
    Coroutine eatCoroutine;
    Coroutine boosterCoroutine;
    Coroutine invulnerableCoroutine;
    Coroutine jumpCoroutine;

    private bool isInvulnerable = false;
    public bool IsInvulnerable => isInvulnerable;

    private void Update()
    {
        hunger.Subtract(hunger.passiveValue * Time.deltaTime);
        stamina.Add(stamina.passiveValue * Time.deltaTime);

        if (hunger.curValue <= 0f)
        {
            health.Subtract(noHungerHealthDecay * Time.deltaTime);
        }

        if (health.curValue <= 0f)
        {
            Die();
        }
    }

    public void Heal(float amount, bool isTic)
    {
        if (isTic)
        {
            if (healCoroutine != null)
                StopCoroutine(healCoroutine);
            healCoroutine = StartCoroutine(health.AddTic(amount));
        }
        else
            health.Add(amount);
    }

    public void Eat(float amount, bool isTic)
    {
        if (isTic)
        {
            if (eatCoroutine != null)
                StopCoroutine(eatCoroutine);
            StartCoroutine(hunger.AddTic(amount));
        }
        else
            hunger.Add(amount);
    }

    public bool UseStamina(float amount)
    {
        if (stamina.curValue - amount < 0)
        {
            return false;
        }
        stamina.Subtract(amount);
        return true;
    }

    public void Booster(float speed)
    {
        CharacterManager.Instance.Player.controller.moveSpeed = speed;
        if (boosterCoroutine != null)
            StopCoroutine(boosterCoroutine);
        boosterCoroutine = StartCoroutine(ResetSpeed(5f));
    }

    public IEnumerator ResetSpeed(float duration)
    {
        yield return new WaitForSeconds(duration);
        CharacterManager.Instance.Player.controller.moveSpeed = 5f;
    }

    public void SetInvulnerability(float time)
    {
        if (invulnerableCoroutine != null)
            StopCoroutine(invulnerableCoroutine);
        invulnerableCoroutine = StartCoroutine(ResetInvulnerability(5f));
    }

    public IEnumerator ResetInvulnerability(float duration)
    {
        isInvulnerable = true;
        yield return new WaitForSeconds(duration);
        isInvulnerable = false;
    }

    public void DoubleJump(float time)
    {
        if (jumpCoroutine != null)
            StopCoroutine(jumpCoroutine);
        jumpCoroutine = StartCoroutine(ResetDoubleJump(time));
    }

    public IEnumerator ResetDoubleJump(float duration)
    {
        CharacterManager.Instance.Player.controller.canDoubleJump = true;
        yield return new WaitForSeconds(duration);
        CharacterManager.Instance.Player.controller.canDoubleJump = false;
    }

    public void Die()
    {
        Debug.Log("플레이어가 죽었다.");
    }

    public void TakePhysicalDamage(int damageAmount)
    {
        if (isInvulnerable) // 무적이면 데미지 감소 로직 실행 X
            return;
        health.Subtract(damageAmount);
        onTakeDamage?.Invoke();
    }
}