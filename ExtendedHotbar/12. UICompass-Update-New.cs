using System;
using UnityEngine;

// Token: 0x0200086A RID: 2154
public partial class UICompass : UIElement
{
	// Token: 0x06003EAF RID: 16047 RVA: 0x0015E398 File Offset: 0x0015C598
	protected new void Update()
	{
		base.Update();
		if (this.TargetTransform)
		{
			this.m_anglePerUnit = this.m_rectTransform.rect.width / this.CompassAngleWidth;
		}
		else if (base.LocalCharacter != null && base.LocalCharacter.CharacterCamera)
		{
			this.TargetTransform = base.LocalCharacter.CharacterCamera.transform;
		}
		// If the StatusEffectPanel has been updated with an icon already
		if (!StatusEffectPanel.IconHeight.Equals(0f))
		{
			// Move the compass down a bit so the icon + timer text does not overlap it anymore
			base.transform.position = new Vector3(base.transform.position.x, (float)Screen.height - StatusEffectPanel.IconHeight * 1.25f, base.transform.position.y);
		}
	}
}
