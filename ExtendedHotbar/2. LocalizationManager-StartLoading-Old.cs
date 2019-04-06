using System;
using System.Collections.Generic;
using System.Xml;
using Localizer;
using UnityEngine;

// Token: 0x020009D8 RID: 2520
public partial class LocalizationManager : MonoBehaviour
{
	// Token: 0x06004A07 RID: 18951 RVA: 0x0018BAB8 File Offset: 0x00189CB8
	private void StartLoading()
	{
		this.m_loading = true;
		this.m_generalLocalization.Clear();
		this.m_itemLocalization.Clear();
		this.m_dialogueLocalization.Clear();
		this.m_loadingTips.Clear();
		this.LoadItemLocalization();
		this.LoadGeneralLocalization();
		this.LoadDialogueLocalization();
	}
}
