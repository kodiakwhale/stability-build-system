using UnityEngine;
using System.IO;

public class GameReader {

	BinaryReader reader;

	public GameReader(BinaryReader _reader) {
		reader = _reader;
	}

	public string ReadString() {
		return reader.ReadString();
	}

	public int ReadInt() {
		return reader.ReadInt32();
	}

	public float ReadFloat() {
		return reader.ReadSingle();
	}

	public Vector3 ReadVector3() {
		Vector3 data;
		data.x = reader.ReadSingle();
		data.y = reader.ReadSingle();
		data.z = reader.ReadSingle();
		return data;
	}

	public Quaternion ReadQuaternion() {
		Quaternion data;
		data.x = reader.ReadSingle();
		data.y = reader.ReadSingle();
		data.z = reader.ReadSingle();
		data.w = reader.ReadSingle();
		return data;
	}
	
	public bool ReadBool() {
		return reader.ReadBoolean();
	}
}