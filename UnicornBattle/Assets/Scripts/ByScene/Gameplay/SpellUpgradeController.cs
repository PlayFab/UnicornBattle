using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UnicornBattle.Controllers
{
	public class SpellUpgradeController : MonoBehaviour
	{
		public Sprite emptyNode;
		public Sprite fullNode;
		public List<Image> nodes = new List<Image>();
		public Transform nodePrefab;

		public void LoadBar(int on, int max)
		{
			if (this.nodes.Count > max)
			{
				int numToRemove = this.nodes.Count - max;
				for (int z = 0; z < numToRemove; z++)
				{
					// just pop the first one off the stack;
					Destroy(nodes[0].gameObject);
					nodes.Remove(nodes[0]);

				}
			}
			else if (this.nodes.Count < max)
			{
				int numToAdd = max - this.nodes.Count;
				for (int z = 0; z < numToAdd; z++)
				{
					Transform newNode = Instantiate<Transform>(this.nodePrefab);
					newNode.SetParent(this.transform, false);
					this.nodes.Add((Image) newNode.GetComponent<Image>());
				}
			}

			int onCount = on;
			//Debug.Log("Node Count: " + nodes.Count);
			foreach (var node in nodes)
			{
				node.overrideSprite = emptyNode;
				if (onCount > 0)
				{
					node.overrideSprite = fullNode;
					onCount--;
				}
			}
		}

	}
}