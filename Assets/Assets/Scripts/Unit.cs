using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class Unit : MonoBehaviour
{

	public string unitName;
	public int points = 10;
	public int unitLevel;
	
	public int damage = 10;
	public int healingPower = 10;
	
	public int maxHP = 100;
	public int currentHP = 100;

	public int maxEnergy = 3;
	public int currentEnergy = 3;

	public int armor = 1;
	public int currentArmor = 1;

	public int criticalChance = 20;
	public int evasionChance = 20;
	public int stunChance = 20;

	public bool stunned = false;
	
	public void setEnergy(int amount)
	{
		currentEnergy = amount;
		if (currentEnergy > maxEnergy)
			currentEnergy = maxEnergy;
	}

	public void GuardUp()
	{
		currentArmor = armor + 10 * unitLevel;
	}

	public void GuardDown()
	{
		currentArmor = armor;
	}
	
	public bool WillCrit()
	{
		int roll = UnityEngine.Random.Range(1, 100);
		Debug.Log("crit: " + roll);
		return roll <= criticalChance;
	}

	public bool WillStun()
	{
		int roll = UnityEngine.Random.Range(1, 100);
		Debug.Log("stun: " + roll);
		return roll <= stunChance;
	}
	
	public bool WillEvade()
	{
		int roll = UnityEngine.Random.Range(1, 100);
		Debug.Log("evade: " + roll);
		return roll <= evasionChance;
	}

	public bool TakeDamage(int dmg)
	{
		currentHP -= (dmg - currentArmor <= 0) ? 0 : dmg - currentArmor;

		if (currentHP <= 0)
			return true;
		else
			return false;
	}

	public void Heal()
	{
		Heal(healingPower);
	}
	
	public void Heal(int amount)
	{
		currentHP += amount;
		if (currentHP > maxHP)
			currentHP = maxHP;
	}

	public Boolean isDead()
	{
		return currentHP <= 0;
	}

}
