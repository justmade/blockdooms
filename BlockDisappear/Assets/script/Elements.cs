using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ElementConfig/Elements")]
public class Elements : ScriptableObject {
	[HideInInspector] public int Unlock = 0;
	 
	[HideInInspector] public int Red = 1;

	[HideInInspector] public int Green = 2;

	[HideInInspector] public int Blue = 3;

	[HideInInspector] public int Yellow = 4;

	[HideInInspector] public int Treasure = 5;

	[HideInInspector] public int Key = 6;

	[HideInInspector] public int White = 7;

	public Material[] _materials;
}
