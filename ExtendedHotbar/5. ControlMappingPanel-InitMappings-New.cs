using System;
using Rewired;
using UnityEngine;

// Token: 0x020008E8 RID: 2280
public partial class ControlMappingPanel : Panel
{
	// Token: 0x0600421C RID: 16924 RVA: 0x001661C4 File Offset: 0x001643C4
	private void InitMappings()
	{
		if (this.m_sectionTemplate)
		{
			this.m_sectionTemplate.gameObject.SetActive(true);
			foreach (InputMapCategory inputMapCategory in ReInput.mapping.UserAssignableMapCategories)
			{
				if (this.ControllerType == ControlMappingPanel.ControlType.Keyboard)
				{
					KeyboardMap keyboardMapInstance = ReInput.mapping.GetKeyboardMapInstance(inputMapCategory.id, 0);
					MouseMap mouseMapInstance = ReInput.mapping.GetMouseMapInstance(inputMapCategory.id, 0);
					// We know this name from debugging, or we can dump it.
					if (inputMapCategory.name == "QuickSlot")
					{
						// Loop through our 8 actions we added via ReWired and create the mapping objects for them.
						for (int i = 0; i < 8; i++)
						{
							var aid = ReInput.mapping.GetActionId(string.Format("QS_Instant{0}", i + 12));
							ElementAssignment elementAssignment = new ElementAssignment(KeyCode.None, ModifierKeyFlags.None, aid, Pole.Positive);
							ActionElementMap actionElementMap;
							keyboardMapInstance.CreateElementMap(elementAssignment, out actionElementMap);
							mouseMapInstance.CreateElementMap(elementAssignment, out actionElementMap);
						}
					}
					this.InitSections(keyboardMapInstance);
					this.InitSections(mouseMapInstance);
				}
				else if (this.m_lastJoystickController != null)
				{
					JoystickMap joystickMapInstance = ReInput.mapping.GetJoystickMapInstance((Joystick)this.m_lastJoystickController, inputMapCategory.id, 0);
					this.InitSections(joystickMapInstance);
				}
				else
				{
					this.m_mappingInitialized = false;
				}
			}
			this.m_sectionTemplate.gameObject.SetActive(false);
		}
	}
}
