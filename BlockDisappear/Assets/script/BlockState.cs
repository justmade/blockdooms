using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[System.Serializable]
public class BlockState{
	public int color;

	public int floor;

	//原始数组的block下标
	public int originalIndex=-1;
	//记录删除是的序列号
	public int step;

	public BlockState(){
	
	}

}
