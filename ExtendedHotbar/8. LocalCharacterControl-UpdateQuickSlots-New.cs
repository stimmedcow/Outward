using System;
using UnityEngine;

// Token: 0x02000481 RID: 1153
public partial class LocalCharacterControl : CharacterControl
{
	// Token: 0x060020B8 RID: 8376 RVA: 0x000C12CC File Offset: 0x000BF4CC
	private void UpdateQuickSlots()
	{
		if (this.m_character != null && this.m_character.QuickSlotMngr != null)
		{
			int playerID = this.m_character.OwnerPlayerSys.PlayerID;
			if (!this.m_character.CharacterUI.IsMenuFocused)
			{
				this.m_character.QuickSlotMngr.ShowQuickSlotSection1 = ControlsInput.QuickSlotToggle1(playerID);
				this.m_character.QuickSlotMngr.ShowQuickSlotSection2 = ControlsInput.QuickSlotToggle2(playerID);
			}
			if (ControlsInput.QuickSlotInstant1(playerID))
			{
				this.m_character.QuickSlotMngr.QuickSlotInput(0);
			}
			else if (ControlsInput.QuickSlotInstant2(playerID))
			{
				this.m_character.QuickSlotMngr.QuickSlotInput(1);
			}
			else if (ControlsInput.QuickSlotInstant3(playerID))
			{
				this.m_character.QuickSlotMngr.QuickSlotInput(2);
			}
			else if (ControlsInput.QuickSlotInstant4(playerID))
			{
				this.m_character.QuickSlotMngr.QuickSlotInput(3);
			}
			else if (ControlsInput.QuickSlotInstant5(playerID))
			{
				this.m_character.QuickSlotMngr.QuickSlotInput(4);
			}
			else if (ControlsInput.QuickSlotInstant6(playerID))
			{
				this.m_character.QuickSlotMngr.QuickSlotInput(5);
			}
			else if (ControlsInput.QuickSlotInstant7(playerID))
			{
				this.m_character.QuickSlotMngr.QuickSlotInput(6);
			}
			else if (ControlsInput.QuickSlotInstant8(playerID))
			{
				this.m_character.QuickSlotMngr.QuickSlotInput(7);
			}
			else
			{
				// Loop through the 8 new slots
				for (var x = 0; x < 8; ++x)
				{
					// See if the action for the slot is pressed
					if(ControlsInput.QuickSlotInstantX(playerID, x + 12))
					{
						// If so, trigger the quick slot. 
						// The index of the slot is 1 less than it's id, as you can see above.
						this.m_character.QuickSlotMngr.QuickSlotInput(x + 11);
						break;
					}
				}
			}
		}
	}
}
