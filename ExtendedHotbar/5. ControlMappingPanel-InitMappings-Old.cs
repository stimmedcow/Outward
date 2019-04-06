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
					this.InitSections(keyboardMapInstance);
					MouseMap mouseMapInstance = ReInput.mapping.GetMouseMapInstance(inputMapCategory.id, 0);
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
