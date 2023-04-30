using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Score", menuName = "Scriptable Objects/Score")]
public class Score : ScriptableObject
{
	[NonSerialized]
	public int CurrentScore;
}
