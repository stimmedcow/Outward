using System;
using UnityEngine;

// Token: 0x02000812 RID: 2066
public partial class KeyboardQuickSlotPanel : QuickSlotPanel
{
	// Token: 0x06003BC3 RID: 15299 RVA: 0x00146CA0 File Offset: 0x00144EA0
	protected override void InitializeQuickSlotDisplays()
	{
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
