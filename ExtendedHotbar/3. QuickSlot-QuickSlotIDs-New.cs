using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020006E9 RID: 1769
public class QuickSlot : MonoBehaviour, ISavable
{
	// Token: 0x06003458 RID: 13400 RVA: 0x0011E598 File Offset: 0x0011C798
	public QuickSlot()
	{
	}

	// Token: 0x17000B12 RID: 2834
	// (get) Token: 0x06003459 RID: 13401 RVA: 0x0011E5A0 File Offset: 0x0011C7A0
	// (set) Token: 0x0600345A RID: 13402 RVA: 0x0011E5A8 File Offset: 0x0011C7A8
	public int ItemID { get; protected set; }

	// Token: 0x17000B13 RID: 2835
	// (get) Token: 0x0600345B RID: 13403 RVA: 0x0011E5B4 File Offset: 0x0011C7B4
	// (set) Token: 0x0600345C RID: 13404 RVA: 0x0011E5BC File Offset: 0x0011C7BC
	public bool ItemCanBeSubstituted { get; protected set; }

	// Token: 0x17000B14 RID: 2836
	// (get) Token: 0x0600345D RID: 13405 RVA: 0x0011E5C8 File Offset: 0x0011C7C8
	// (set) Token: 0x0600345E RID: 13406 RVA: 0x0011E5D0 File Offset: 0x0011C7D0
	public bool ItemIsSkill { get; protected set; }

	// Token: 0x17000B15 RID: 2837
	// (get) Token: 0x0600345F RID: 13407 RVA: 0x0011E5DC File Offset: 0x0011C7DC
	// (set) Token: 0x06003460 RID: 13408 RVA: 0x0011E5E4 File Offset: 0x0011C7E4
	public bool ItemIsEquipment { get; protected set; }

	// Token: 0x17000B16 RID: 2838
	// (get) Token: 0x06003461 RID: 13409 RVA: 0x0011E5F0 File Offset: 0x0011C7F0
	// (set) Token: 0x06003462 RID: 13410 RVA: 0x0011E5F8 File Offset: 0x0011C7F8
	public Skill ItemAsSkill { get; protected set; }

	// Token: 0x06003463 RID: 13411 RVA: 0x0011E604 File Offset: 0x0011C804
	private void Awake()
	{
		this.ItemID = -1;
	}

	// Token: 0x06003464 RID: 13412 RVA: 0x0011E610 File Offset: 0x0011C810
	public void SetOwner(Character _owner)
	{
		this.m_owner = _owner;
	}

	// Token: 0x06003465 RID: 13413 RVA: 0x0011E61C File Offset: 0x0011C81C
	public void Activate()
	{
		this.CheckAndUpdateRefItem();
		if (!this.ActiveItem)
		{
			return;
		}
		if (this.ActiveItem != null && (this.m_owner.Inventory.OwnsItem(this.ActiveItem.UID) || (this.ItemIsSkill && this.m_owner.Inventory.LearnedSkill(this.ItemAsSkill))))
		{
			if (this.m_activeItem != this.m_registeredItem)
			{
				this.SetQuickSlot(this.m_activeItem, false);
			}
			this.ActiveItem.QuickSlotUse();
		}
		else if (this.m_owner && this.m_owner.IsLocalPlayer && this.m_owner.CharacterUI)
		{
			this.m_owner.CharacterUI.ShowInfoNotification(LocalizationManager.Instance.GetLoc("Notification_Quickslot_NoItemInventory", new string[]
			{
				Global.SetTextColor(this.ActiveItem.Name, Global.HIGHLIGHT)
			}));
		}
	}

	// Token: 0x06003466 RID: 13414 RVA: 0x0011E73C File Offset: 0x0011C93C
	public void CheckAndUpdateRefItem()
	{
		this.CheckAndUpdateRefItem(null);
	}

	// Token: 0x06003467 RID: 13415 RVA: 0x0011E748 File Offset: 0x0011C948
	public void CheckAndUpdateRefItem(IList<string> _itemUIDsToIgnore)
	{
		this.m_checkAndUpdateRefWanted = false;
		bool flag = this.CheckItemOwnership(this.m_registeredItem, _itemUIDsToIgnore);
		if (!flag)
		{
			if (this.m_activeItem == this.m_registeredItem)
			{
				this.m_activeItem = null;
			}
		}
		else if (this.m_activeItem != this.m_registeredItem)
		{
			this.SetActiveItem(this.m_registeredItem);
		}
		if (!flag && this.ItemCanBeSubstituted && this.ItemID != -1)
		{
			Item item = null;
			if (!this.ItemIsSkill)
			{
				if (item == null && this.m_owner.Inventory.HasABag)
				{
					List<string> containedItemUIDs = this.m_owner.Inventory.EquippedBag.Container.GetContainedItemUIDs(this.ItemID);
					for (int i = 0; i < containedItemUIDs.Count; i++)
					{
						if (_itemUIDsToIgnore == null || !_itemUIDsToIgnore.Contains(containedItemUIDs[i]))
						{
							item = ItemManager.Instance.GetItem(containedItemUIDs[i]);
							if (item != null)
							{
								break;
							}
						}
					}
				}
				if (item == null)
				{
					List<string> containedItemUIDs = this.m_owner.Inventory.Pouch.GetContainedItemUIDs(this.ItemID);
					for (int j = 0; j < containedItemUIDs.Count; j++)
					{
						if (_itemUIDsToIgnore == null || !_itemUIDsToIgnore.Contains(containedItemUIDs[j]))
						{
							item = ItemManager.Instance.GetItem(containedItemUIDs[j]);
							if (item != null)
							{
								break;
							}
						}
					}
				}
				if (item == null)
				{
					item = ResourcesPrefabManager.Instance.GetItemPrefab(this.ItemID);
				}
			}
			else
			{
				item = this.m_owner.Inventory.SkillKnowledge.GetItemFromItemID(this.ItemID);
			}
			if (item != null)
			{
				this.SetActiveItem(item);
			}
		}
	}

	// Token: 0x06003468 RID: 13416 RVA: 0x0011E948 File Offset: 0x0011CB48
	private bool CheckItemOwnership(Item _item, IList<string> _itemUIDsToIgnore)
	{
		bool flag = _item != null;
		if (flag && _itemUIDsToIgnore != null && _itemUIDsToIgnore.Contains(_item.UID))
		{
			flag = false;
		}
		if (flag && _item != null)
		{
			if (!(_item is Skill))
			{
				flag = this.m_owner.Inventory.OwnsItem(_item.UID);
			}
			else
			{
				flag = this.m_owner.Inventory.SkillKnowledge.IsItemLearned(_item.ItemID);
			}
		}
		return flag;
	}

	// Token: 0x06003469 RID: 13417 RVA: 0x0011E9D4 File Offset: 0x0011CBD4
	internal void ItemAddedToInventory(Item _itemAdded)
	{
		if ((this.m_activeItem == null || string.IsNullOrEmpty(this.m_activeItem.UID)) && this.ItemCanBeSubstituted && this.ItemID != -1 && _itemAdded.ItemID == this.ItemID)
		{
			this.SetActiveItem(_itemAdded);
		}
	}

	// Token: 0x0600346A RID: 13418 RVA: 0x0011EA38 File Offset: 0x0011CC38
	public void SetQuickSlot(Item _item, bool _forceQuickSlot = false)
	{
		if (_item == null)
		{
			this.Clear();
			return;
		}
		this.ItemCanBeSubstituted = (_item.CanBeSubstitutedInQuickSlot || _item is Skill);
		this.ItemID = _item.ItemID;
		this.SetActiveItem(_item);
		this.ItemIsEquipment = (this.m_activeItem is Equipment);
		this.ItemIsSkill = (this.m_activeItem is Skill);
		if (this.m_activeItem != null && this.ItemIsSkill)
		{
			this.ItemAsSkill = (this.m_activeItem as Skill);
		}
		else
		{
			this.ItemAsSkill = null;
		}
		if (!_item.HolderUID.IsNull || _forceQuickSlot)
		{
			this.m_registeredItemUID = _item.UID;
			this.m_registeredItem = _item;
			this.m_registeredItem.SetQuickSlot(this.Index);
		}
	}

	// Token: 0x0600346B RID: 13419 RVA: 0x0011EB2C File Offset: 0x0011CD2C
	private void SetActiveItem(Item _item)
	{
		if (this.m_activeItem && this.m_activeItem.QuickSlotIndex == this.Index)
		{
			this.m_activeItem.SetQuickSlot(-1);
		}
		this.m_activeItem = _item;
		this.ItemIsSkill = false;
		this.ItemAsSkill = null;
		this.ItemIsEquipment = false;
		if (this.m_activeItem)
		{
			this.m_activeItem.SetQuickSlot(this.Index);
		}
		if (this.RefreshCallback != null)
		{
			this.RefreshCallback();
		}
	}

	// Token: 0x0600346C RID: 13420 RVA: 0x0011EBC0 File Offset: 0x0011CDC0
	public void Clear()
	{
		if (this.ActiveItem != null && !string.IsNullOrEmpty(this.ActiveItem.UID))
		{
			if (this.ActiveItem.QuickSlotIndex == this.Index)
			{
				this.ActiveItem.SetQuickSlot(-1);
			}
			else
			{
				Debug.LogError("The item " + this.ActiveItem + " had the wrong Quickslot ID!");
			}
		}
		this.SetActiveItem(null);
		this.m_registeredItem = null;
		this.m_registeredItemUID = UID.Empty;
		this.ItemCanBeSubstituted = false;
		this.ItemID = -1;
	}

	// Token: 0x0600346D RID: 13421 RVA: 0x0011EC5C File Offset: 0x0011CE5C
	public void ItemDestroyed(Item _destroyedItem)
	{
		if (_destroyedItem)
		{
			this.CheckAndUpdateRefItem(new string[]
			{
				_destroyedItem.UID
			});
		}
	}

	// Token: 0x0600346E RID: 13422 RVA: 0x0011EC80 File Offset: 0x0011CE80
	public void ItemMovedInInventory(Item _movedItem)
	{
		if (_movedItem == null || _movedItem == this.m_registeredItem)
		{
			return;
		}
		if (!this.m_checkAndUpdateRefWanted)
		{
			base.Invoke("CheckAndUpdateRefItem", 0.1f);
		}
		this.m_checkAndUpdateRefWanted = true;
	}

	// Token: 0x0600346F RID: 13423 RVA: 0x0011ECD0 File Offset: 0x0011CED0
	public string ToSaveData()
	{
		string text = string.Empty;
		for (int i = 0; i < 4; i++)
		{
			text = text + this.GetSaveData((QuickSlot.SaveDataType)i) + ";";
		}
		return text;
	}

	// Token: 0x06003470 RID: 13424 RVA: 0x0011ED0C File Offset: 0x0011CF0C
	private string GetSaveData(QuickSlot.SaveDataType _dataType)
	{
		string result = string.Empty;
		switch (_dataType)
		{
		case QuickSlot.SaveDataType.SlotID:
			result = this.Index.ToString();
			break;
		case QuickSlot.SaveDataType.ItemID:
			result = this.ItemID.ToString();
			break;
		case QuickSlot.SaveDataType.RefItem:
			result = this.m_registeredItemUID;
			break;
		case QuickSlot.SaveDataType.CanBeSubst:
			result = this.ItemCanBeSubstituted.ToString();
			break;
		}
		return result;
	}

	// Token: 0x06003471 RID: 13425 RVA: 0x0011ED9C File Offset: 0x0011CF9C
	public void LoadSaveData(string _saveData)
	{
		string[] array = _saveData.Split(new char[]
		{
			';'
		});
		int itemID = 0;
		int.TryParse(array[1], out itemID);
		this.ItemID = itemID;
		bool itemCanBeSubstituted = false;
		bool.TryParse(array[3], out itemCanBeSubstituted);
		this.ItemCanBeSubstituted = itemCanBeSubstituted;
		Item item = null;
		bool flag = false;
		if (!string.IsNullOrEmpty(array[2]))
		{
			this.m_registeredItemUID = array[2];
			this.m_registeredItem = ItemManager.Instance.GetItem(this.m_registeredItemUID);
			item = this.m_registeredItem;
		}
		if (item == null && this.ItemID != 0 && this.ItemID != -1)
		{
			item = ResourcesPrefabManager.Instance.GetItemPrefab(this.ItemID);
			if (item is Skill)
			{
				this.ItemCanBeSubstituted = true;
				Item itemFromItemID = this.m_owner.Inventory.SkillKnowledge.GetItemFromItemID(this.ItemID);
				if (itemFromItemID)
				{
					item = itemFromItemID;
				}
			}
			flag = true;
		}
		if (item)
		{
			this.SetQuickSlot(item, false);
		}
		else if (flag)
		{
			this.CheckAndUpdateRefItem();
		}
	}

	// Token: 0x17000B17 RID: 2839
	// (get) Token: 0x06003472 RID: 13426 RVA: 0x0011EEC0 File Offset: 0x0011D0C0
	public object SaveIdentifier
	{
		get
		{
			return this.Index;
		}
	}

	// Token: 0x17000B18 RID: 2840
	// (get) Token: 0x06003473 RID: 13427 RVA: 0x0011EED0 File Offset: 0x0011D0D0
	public Character OwnerCharacter
	{
		get
		{
			return this.m_owner;
		}
	}

	// Token: 0x17000B19 RID: 2841
	// (get) Token: 0x06003474 RID: 13428 RVA: 0x0011EED8 File Offset: 0x0011D0D8
	public bool IsFree
	{
		get
		{
			return this.m_registeredItemUID.IsNull;
		}
	}

	// Token: 0x17000B1A RID: 2842
	// (get) Token: 0x06003475 RID: 13429 RVA: 0x0011EEE8 File Offset: 0x0011D0E8
	public Item ActiveItem
	{
		get
		{
			return this.m_activeItem;
		}
	}

	// Token: 0x17000B1B RID: 2843
	// (get) Token: 0x06003476 RID: 13430 RVA: 0x0011EEF0 File Offset: 0x0011D0F0
	public Item RegisteredItem
	{
		get
		{
			return this.m_registeredItem;
		}
	}

	// Token: 0x17000B1C RID: 2844
	// (get) Token: 0x06003477 RID: 13431 RVA: 0x0011EEF8 File Offset: 0x0011D0F8
	public bool HasSubstitute
	{
		get
		{
			return this.ItemID != -1 && this.m_activeItem != this.m_registeredItem;
		}
	}

	// Token: 0x17000B1D RID: 2845
	// (get) Token: 0x06003478 RID: 13432 RVA: 0x0011EF1C File Offset: 0x0011D11C
	public bool IsLocked
	{
		get
		{
			if (this.ItemIsSkill)
			{
				return !this.ItemAsSkill.IsChildToCharacter || !this.ItemAsSkill.HasBaseRequirements(false);
			}
			return this.ActiveItem == null || (this.ItemIsEquipment && this.m_activeItem.IsEquipped) || !this.m_owner.Inventory.OwnsItem(this.ActiveItem.ItemID);
		}
	}

	// Token: 0x04002FD1 RID: 12241
	public int Index;

	// Token: 0x04002FD2 RID: 12242
	public bool ItemQuickSlot;

	// Token: 0x04002FD5 RID: 12245
	private UID m_registeredItemUID;

	// Token: 0x04002FD6 RID: 12246
	private Item m_registeredItem;

	// Token: 0x04002FD7 RID: 12247
	private Item m_activeItem;

	// Token: 0x04002FDB RID: 12251
	private Character m_owner;

	// Token: 0x04002FDC RID: 12252
	public UnityAction RefreshCallback;

	// Token: 0x04002FDD RID: 12253
	private bool m_checkAndUpdateRefWanted;

	// Token: 0x020006EA RID: 1770
	private enum SaveDataType
	{
		// Token: 0x04002FDF RID: 12255
		SlotID,
		// Token: 0x04002FE0 RID: 12256
		ItemID,
		// Token: 0x04002FE1 RID: 12257
		RefItem,
		// Token: 0x04002FE2 RID: 12258
		CanBeSubst,
		// Token: 0x04002FE3 RID: 12259
		Count
	}

	// Token: 0x020006EB RID: 1771
	public enum QuickSlotIDs
	{
		// Token: 0x04002FE5 RID: 12261
		None,
		// Token: 0x04002FE6 RID: 12262
		RT_A,
		// Token: 0x04002FE7 RID: 12263
		RT_B,
		// Token: 0x04002FE8 RID: 12264
		RT_X,
		// Token: 0x04002FE9 RID: 12265
		RT_Y,
		// Token: 0x04002FEA RID: 12266
		LT_A,
		// Token: 0x04002FEB RID: 12267
		LT_B,
		// Token: 0x04002FEC RID: 12268
		LT_X,
		// Token: 0x04002FED RID: 12269
		LT_Y,
		// Token: 0x04002FEE RID: 12270
		Item1,
		// Token: 0x04002FEF RID: 12271
		Item2,
		// Token: 0x04002FF0 RID: 12272
		Item3,
		Item4,
		Item5,
		Item6,
		Item7,
		Item8,
		Item9,
		Item10,
		Item11
	}
}
