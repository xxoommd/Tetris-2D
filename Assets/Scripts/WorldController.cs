using UnityEngine;
using System.Collections;
using System.Collections.Generic;


[System.Serializable]
public class LevelData
{
	public ushort level;
	public byte weightI;
	public byte weightJ;
	public byte weightL;
	public byte weightS;
	public byte weightZ;
	public byte weightO;
	public byte weightT;

	public ushort limitTime;	// seconds
	public ushort aimClearings;
}

[System.Serializable]
public class WorldData
{
	public LevelData[] levels;
}

public class WorldController : Singleton <WorldController>
{
	public WorldData[] worlds;

	public WorldData FindWorldData (ushort world)
	{
		return worlds [world];
	}

	public LevelData FindLevelData (ushort world, ushort level)
	{
		return worlds [world].levels [level];
	}
}
