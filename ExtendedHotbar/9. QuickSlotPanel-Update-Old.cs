using System;
using UnityEngine;

// Token: 0x020008F4 RID: 2292
public partial class QuickSlotPanel : UIElement
{
	// Token: 0x06004291 RID: 17041 RVA: 0x0016CE8C File Offset: 0x0016B08C
	protected new void Update()
	{
		base.Update();
		if ((base.LocalCharacter == null || this.m_lastCharacter != base.LocalCharacter) && this.m_initialized)
		{
			this.m_initialized = false;
		}
		if (this.m_initialized)
		{
			if (this.UpdateInputVisibility)
			{
				for (int i = 0; i < this.m_quickSlotDisplays.Length; i++)
				{
					this.m_quickSlotDisplays[i].SetInputTargetAlpha((float)((!this.m_active) ? 0 : 1));
				}
			}
		}
		else if (base.LocalCharacter != null)
		{
			for (int j = 0; j < this.m_quickSlotDisplays.Length; j++)
			{
				int refSlotID = this.m_quickSlotDisplays[j].RefSlotID;
				this.m_quickSlotDisplays[j].SetQuickSlot(base.LocalCharacter.QuickSlotMngr.GetQuickSlot(refSlotID));
			}
			this.m_lastCharacter = base.LocalCharacter;
			this.m_initialized = true;
		}
	}
}
