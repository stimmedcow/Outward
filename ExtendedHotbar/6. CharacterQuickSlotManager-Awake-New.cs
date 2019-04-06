using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000464 RID: 1124
public partial class CharacterQuickSlotManager : MonoBehaviour
{
	// Token: 0x06001EAD RID: 7853 RVA: 0x000B64DC File Offset: 0x000B46DC
	private void Awake()
	{
		this.m_character = base.GetComponent<Character>();
		this.m_quickslotTrans = base.transform.Find("QuickSlots");
		// Add our 8 QuickSlots
		for(var x = 0; x < 8; ++x)
		{
			// Create a GameObject with a name that shouldn't overlap with anything else
			GameObject gameObject = new GameObject(string.Format("EXQS_{0}", x));
			// Add a QuickSlot object and set the name based on how the client will process it
			QuickSlot qs = gameObject.AddComponent<QuickSlot>();
			qs.name = string.Format("{0}", x + 12);
			// Set the parent so the code below will find the new objects and treat them like the originals
			gameObject.transform.SetParent(this.m_quickslotTrans);
		}
		QuickSlot[] componentsInChildren = this.m_quickslotTrans.GetComponentsInChildren<QuickSlot>();
		this.m_quickSlots = new QuickSlot[componentsInChildren.Length];
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			int num = int.Parse(componentsInChildren[i].name);
			this.m_quickSlots[num - 1] = componentsInChildren[i];
			this.m_quickSlots[num - 1].Index = num - 1;
		}
		for (int j = 0; j < this.m_quickSlots.Length; j++)
		{
			this.m_quickSlots[j].SetOwner(this.m_character);
		}
	}
}
