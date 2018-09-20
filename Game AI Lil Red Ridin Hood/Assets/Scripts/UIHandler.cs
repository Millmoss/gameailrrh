using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHandler : MonoBehaviour
{
	private Queue<string> lines;
	public Text text;

	void Start ()
	{
		lines = new Queue<string>();
	}
	
	void Update ()
	{
		string t = "";
		for (int i = 0; i < lines.Count; i++)
		{
			string l = lines.Dequeue();
			lines.Enqueue(l);
			t += l + "\n";
		}
		text.text = t;
	}

	public void addLine(string line)
	{
		lines.Enqueue(line);
		if (lines.Count > 8)
			lines.Dequeue();
	}
}
