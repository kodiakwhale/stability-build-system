using UnityEngine;
using System.IO;

public class GameWriter {

	BinaryWriter writer;

	public GameWriter (BinaryWriter _writer) {
		writer = _writer;
	}

	public void Write (string data) {
		writer.Write(data);
	}

	public void Write (int data) {
		writer.Write(data);
	}

	public void Write (float data) {
		writer.Write(data);
	}

	public void Write (Vector3 data) {
		writer.Write(data.x);
		writer.Write(data.y);
		writer.Write(data.z);
	}

	public void Write (Quaternion data) {
		writer.Write(data.x);
		writer.Write(data.y);
		writer.Write(data.z);
		writer.Write(data.w);
	}
	
	public void Write (bool data) {
		writer.Write(data);
	}
}
