using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000854 RID: 2132
public class StatusEffectIcon : UIElement
{
	// Token: 0x06003E34 RID: 15924 RVA: 0x00018EBF File Offset: 0x000170BF
	public StatusEffectIcon()
	{
	}

	// Token: 0x06003E35 RID: 15925 RVA: 0x0002D018 File Offset: 0x0002B218
	protected override void AwakeInit()
	{
		base.AwakeInit();
		if (this.m_icon == null)
		{
			this.m_icon = base.GetComponent<Image>();
		}

		// Create the TextGenerator once
		m_textGenerator = new TextGenerator();

		// Create a new Text label for the timer
		this.m_lblTimer = UnityEngine.Object.Instantiate<Text>(this.m_lblStack);
		this.m_lblTimer.transform.SetParent(base.transform);
		this.m_lblTimer.transform.ResetLocal(true);
	}

	// Token: 0x06003E36 RID: 15926 RVA: 0x0002D03D File Offset: 0x0002B23D
	protected override void StartInit()
	{
		base.StartInit();
		if (this.m_recedingIcon)
		{
			this.m_recedingIcon.gameObject.SetActive(false);
		}
	}

	// Token: 0x06003E37 RID: 15927 RVA: 0x0015BE98 File Offset: 0x0015A098
	protected new void Update()
	{
		base.Update();
		this.m_lblStack.text = ((this.m_currentStack <= 1) ? string.Empty : this.m_currentStack.ToString());
		this.m_lblTimer.text = "";
		// If the effect is set and is not permanent
		if (this.m_effect != null && !this.m_effect.Permanent)
		{
			// Show the lifespan in the format of 00:0
			TimeSpan timeSpan = TimeSpan.FromSeconds((double)this.m_effect.RemainingLifespan);
			this.m_lblTimer.text = string.Format("{0:D2}:{1:D2}", timeSpan.Minutes, timeSpan.Seconds);
			// We want to center the text under the icon, sp get the width of the string, and then adjust the local position.
			TextGenerationSettings generationSettings = this.m_lblTimer.GetGenerationSettings(this.m_lblTimer.rectTransform.rect.size);
			float preferredWidth = m_textGenerator.GetPreferredWidth(this.m_lblTimer.text, generationSettings);
			this.m_lblTimer.transform.localPosition = new Vector3(-preferredWidth / 2f, 0f, 0f);
		}
		if (this.m_currentStack == 0)
		{
			base.Hide();
		}
	}

	// Token: 0x06003E38 RID: 15928 RVA: 0x0002D066 File Offset: 0x0002B266
	public void SetIcon(Sprite _icon)
	{
		if (this.m_icon)
		{
			this.m_icon.overrideSprite = _icon;
		}
	}

	// Token: 0x06003E39 RID: 15929 RVA: 0x0002D084 File Offset: 0x0002B284
	public void SetReceding()
	{
		if (this.m_recedingIcon)
		{
			this.m_recedingIcon.gameObject.SetActive(true);
		}
	}

	// Token: 0x06003E3A RID: 15930 RVA: 0x0002D0A7 File Offset: 0x0002B2A7
	public void IncreaseStack(int _amount)
	{
		this.m_currentStack += _amount;
		if (!this.IsDisplayed)
		{
			this.Show();
		}
	}

	// Token: 0x06003E3B RID: 15931 RVA: 0x0015BEF0 File Offset: 0x0015A0F0
	public void ResetStack()
	{
		if (this.m_recedingIcon)
		{
			this.m_recedingIcon.gameObject.SetActive(false);
		}
		this.m_currentStack = 0;
		if (this.m_lblStack)
		{
			this.m_lblStack.text = string.Empty;
		}
	}

	public void SetEffect(StatusEffect effect)
	{
		this.m_effect = effect;
	}

	// Token: 0x0400387A RID: 14458
	[SerializeField]
	private Image m_icon;

	// Token: 0x0400387B RID: 14459
	[SerializeField]
	private Text m_lblStack;

	// Token: 0x0400387C RID: 14460
	[SerializeField]
	private Image m_recedingIcon;

	// Token: 0x0400387D RID: 14461
	private int m_currentStack;

	[SerializeField]
	private Text m_lblTimer;

	// The effect this icon represents
	private StatusEffect m_effect;

	// TextGenerator to get the display width of the string we add
	private TextGenerator m_textGenerator;
}
