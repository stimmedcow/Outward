using System;
using UnityEngine;

// Token: 0x02000812 RID: 2066
public partial class KeyboardQuickSlotPanel : QuickSlotPanel
{
	// Token: 0x06003BC3 RID: 15299 RVA: 0x00146CA0 File Offset: 0x00144EA0
	protected override void InitializeQuickSlotDisplays()
	{
		// Add 8 new quickslot ids to the array and assign them the new ids we added.
		// Ideally, we could make this a loop and cast away the enum, but it's fine for now;
		// we'll eventually have this logic not hardcoded anymore.
		Array.Resize(ref this.DisplayOrder, this.DisplayOrder.Length + 8);
		this.DisplayOrder[this.DisplayOrder.Length - 8] = QuickSlot.QuickSlotIDs.Item4;
		this.DisplayOrder[this.DisplayOrder.Length - 7] = QuickSlot.QuickSlotIDs.Item5;
		this.DisplayOrder[this.DisplayOrder.Length - 6] = QuickSlot.QuickSlotIDs.Item6;
		this.DisplayOrder[this.DisplayOrder.Length - 5] = QuickSlot.QuickSlotIDs.Item7;
		this.DisplayOrder[this.DisplayOrder.Length - 4] = QuickSlot.QuickSlotIDs.Item8;
		this.DisplayOrder[this.DisplayOrder.Length - 3] = QuickSlot.QuickSlotIDs.Item9;
		this.DisplayOrder[this.DisplayOrder.Length - 2] = QuickSlot.QuickSlotIDs.Item10;
		this.DisplayOrder[this.DisplayOrder.Length - 1] = QuickSlot.QuickSlotIDs.Item11;
		
		EditorQuickSlotDisplayPlacer componentInChildren = base.GetComponentInChildren<EditorQuickSlotDisplayPlacer>();
		this.m_quickSlotDisplays = new QuickSlotDisplay[this.DisplayOrder.Length];
		for (int i = 0; i < this.m_quickSlotDisplays.Length; i++)
		{
			EditorQuickSlotDisplayPlacer editorQuickSlotDisplayPlacer = UnityEngine.Object.Instantiate<EditorQuickSlotDisplayPlacer>(componentInChildren);
			editorQuickSlotDisplayPlacer.IsTemplate = false;
			editorQuickSlotDisplayPlacer.transform.SetParent(componentInChildren.transform.parent);
			editorQuickSlotDisplayPlacer.transform.ResetLocal(true);
			editorQuickSlotDisplayPlacer.RefSlotID = this.DisplayOrder[i] - QuickSlot.QuickSlotIDs.RT_A;
			editorQuickSlotDisplayPlacer.Init();
			this.m_quickSlotDisplays[i] = editorQuickSlotDisplayPlacer.SlotDisplay;
			if (this.m_overrideInputIconPos)
			{
				this.m_quickSlotDisplays[i].SetOverrideInputPos(this.m_inputPos);
			}
		}
		componentInChildren.gameObject.SetActive(false);
	}
}
