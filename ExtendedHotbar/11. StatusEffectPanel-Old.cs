using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000855 RID: 2133
public class StatusEffectPanel : UIElement
{
	// Token: 0x06003E3D RID: 15933 RVA: 0x0002D0DB File Offset: 0x0002B2DB
	private void Awake()
	{
		this.m_statusPrefab = base.GetComponentInChildren<StatusEffectIcon>();
		this.m_statusPrefab.Hide();
	}

	// Token: 0x06003E3E RID: 15934 RVA: 0x0015BF48 File Offset: 0x0015A148
	protected new void Update()
	{
		base.Update();
		this.ResetStatusIcons();
		if (base.LocalCharacter.StatusEffectMngr != null)
		{
			for (int i = 0; i < base.LocalCharacter.StatusEffectMngr.Statuses.Count; i++)
			{
				this.m_cachedStatus = base.LocalCharacter.StatusEffectMngr.Statuses[i];
				this.m_cachedDisease = (this.m_cachedStatus as Disease);
				if (this.m_cachedStatus != null && this.m_cachedStatus.IsActive && this.m_cachedStatus.StatusIcon != null)
				{
					string statusIconIdentifier = this.m_cachedStatus.StatusIconIdentifier;
					if (!string.IsNullOrEmpty(statusIconIdentifier))
					{
						StatusEffectIcon statusIcon = this.GetStatusIcon(statusIconIdentifier);
						statusIcon.SetIcon(this.m_cachedStatus.StatusIcon);
						statusIcon.IncreaseStack(this.m_cachedStatus.StackCount);
						if (this.m_cachedDisease && this.m_cachedDisease.IsReceding)
						{
							statusIcon.SetReceding();
						}
					}
				}
			}
		}
		if (base.LocalCharacter.CurrentWeapon)
		{
			if (base.LocalCharacter.CurrentWeapon.IsSummonedEquipment)
			{
				StatusEffectIcon statusIcon2 = this.GetStatusIcon("SummonWeapon");
				statusIcon2.SetIcon(base.LocalCharacter.CurrentWeapon.SummonedEquipment.StatusIcon);
				statusIcon2.IncreaseStack(1);
			}
			if (base.LocalCharacter.CurrentWeapon.Imbued)
			{
				StatusEffectIcon statusIcon3 = this.GetStatusIcon("ImbueMainWeapon");
				statusIcon3.SetIcon(base.LocalCharacter.CurrentWeapon.FirstImbue.ImbuedEffectPrefab.ImbueStatusIcon);
				statusIcon3.IncreaseStack(1);
			}
		}
		if (base.LocalCharacter.LeftHandWeapon && base.LocalCharacter.LeftHandWeapon != base.LocalCharacter.CurrentWeapon && base.LocalCharacter.LeftHandWeapon.Imbued)
		{
			StatusEffectIcon statusIcon4 = this.GetStatusIcon("ImbueOffWeapon");
			statusIcon4.SetIcon(base.LocalCharacter.LeftHandWeapon.FirstImbue.ImbuedEffectPrefab.ImbueStatusIcon);
			statusIcon4.IncreaseStack(1);
		}
		if (base.LocalCharacter.CurrentSummon && !base.LocalCharacter.CurrentSummon.IsDead)
		{
			StatusEffectIcon statusIcon5 = this.GetStatusIcon("SummonGhost");
			statusIcon5.SetIcon(UIUtilities.SummonGhostStatusIcon);
			statusIcon5.IncreaseStack(1);
		}
	}

	// Token: 0x06003E3F RID: 15935 RVA: 0x0015C1D8 File Offset: 0x0015A3D8
	private StatusEffectIcon GetStatusIcon(string statusIconKey)
	{
		StatusEffectIcon statusEffectIcon = null;
		if (!this.m_statusIcons.TryGetValue(statusIconKey, out statusEffectIcon))
		{
			statusEffectIcon = UnityEngine.Object.Instantiate<StatusEffectIcon>(this.m_statusPrefab);
			statusEffectIcon.transform.SetParent(base.transform);
			statusEffectIcon.transform.ResetLocal(true);
			statusEffectIcon.gameObject.SetActive(true);
			this.m_statusIcons.Add(statusIconKey, statusEffectIcon);
		}
		return statusEffectIcon;
	}

	// Token: 0x06003E40 RID: 15936 RVA: 0x0015C240 File Offset: 0x0015A440
	private void ResetStatusIcons()
	{
		foreach (string key in this.m_statusIcons.Keys)
		{
			this.m_statusIcons[key].ResetStack();
		}
	}

	// Token: 0x0400387E RID: 14462
	private StatusEffectIcon m_statusPrefab;

	// Token: 0x0400387F RID: 14463
	private Dictionary<string, StatusEffectIcon> m_statusIcons = new Dictionary<string, StatusEffectIcon>();

	// Token: 0x04003880 RID: 14464
	private StatusEffect m_cachedStatus;

	// Token: 0x04003881 RID: 14465
	private Disease m_cachedDisease;
}
