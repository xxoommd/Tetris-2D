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
public struct WorldData
{
	public LevelData[] levels;
}

public class WorldController : Singleton <WorldController>
{
	public WorldData[] worlds;

	public LevelData FindLevelData(ushort worldID, ushort level) {
		return worlds [worldID].levels [level];
	}
}
