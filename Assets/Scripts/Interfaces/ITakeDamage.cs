using UnityEngine;
using System.Collections;
/*
	ITakeDamage.cs
	Interface for implementing damageable objects
*/

public interface ITakeDamage<T>
{
	void Damage(T damageTaken);
}
