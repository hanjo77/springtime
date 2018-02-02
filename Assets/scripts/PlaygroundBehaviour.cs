using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using System.IO;

public class PlaygroundBehaviour : MonoBehaviour {

	public Transform grassBlock;

	// Use this for initialization
	void Start () {
		BuildLevel ("test.txt");
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void BuildLevel(string fileName) {
		string path = "Assets/levels/" + fileName;

		using (StreamReader sr = new StreamReader(path)) 
		{
			int lineCount = 0;
			while (sr.Peek() >= 0) 
			{
				int charCount = 0;

				foreach (char c in sr.ReadLine())
				{
					int j;
					if (Int32.TryParse (c.ToString (), out j)) {
						Instantiate(grassBlock, new Vector3(charCount, j, -lineCount), Quaternion.identity);
					}
					charCount++;
				}
				lineCount++;
			}
		}
	}
}
