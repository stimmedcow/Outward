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
			// We want to find a specific QuickSlotPanel instance, since there are multiple
			if (base.name == "Keyboard" && base.transform.parent.name == "QuickSlot")
			{
				// Find our default StabilityDisplay_Simple object
				StabilityDisplay_Simple stabilityDisplay = UnityEngine.Object.FindObjectOfType<StabilityDisplay_Simple>();
				// Streamline the stability display so it's not so far from the bottom of the screen.
				// This also means the hotbar gets placed closer to the bottom of the screen, but
				// still with neat spacing.
				stabilityDisplay.transform.position = new Vector3(stabilityDisplay.transform.position.x, stabilityDisplay.transform.position.y / 4f, stabilityDisplay.transform.position.z);
				// Get the screen coords of its corners
				Vector3[] stabilityDisplayCorners = new Vector3[4];
				stabilityDisplay.RectTransform.GetWorldCorners(stabilityDisplayCorners);
				// We want to set the QuickSlotPanel to be above the stability display, with equal space above the 
				// stability display as there is below it, to make it look nice and neat.
				float newY = stabilityDisplayCorners[1].y + stabilityDisplayCorners[0].y;
				base.transform.parent.position = new Vector3(base.transform.parent.position.x, newY, base.transform.parent.position.z);
				// Logic to center the panel so the look and feel is more consistent with normal games
				bool centerBar = true;
				if (centerBar)
				{
					Vector3[] v0 = new Vector3[4];
					Vector3[] v1 = new Vector3[4];
					this.m_quickSlotDisplays[0].RectTransform.GetWorldCorners(v0);
					this.m_quickSlotDisplays[1].RectTransform.GetWorldCorners(v1);
					// The width of each icon
					var qsdWidth = v0[2].x - v0[1].x;
					// The width of the space between each icon
					var qsdSpacer = v1[0].x - v0[2].x;
					// Total space per icon/spacer pair
					var elemWidth = qsdWidth + qsdSpacer;
					// How long our bar really is
					var realWidth = elemWidth * this.m_quickSlotDisplays.Length;
					// Re-center it based on actual content
					base.transform.parent.position = new Vector3(realWidth / 2.0f + elemWidth / 2.0f, base.transform.parent.position.y, base.transform.parent.position.z);
				}
			}
		}
	}
}
