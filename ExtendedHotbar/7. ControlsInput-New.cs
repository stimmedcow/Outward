using System;
using System.Collections.Generic;
using Rewired;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020004BA RID: 1210
public class ControlsInput : MonoBehaviour
{
	// Token: 0x060022B0 RID: 8880 RVA: 0x000CBFC8 File Offset: 0x000CA1C8
	public ControlsInput()
	{
	}

	// Token: 0x060022B1 RID: 8881 RVA: 0x000CBFD0 File Offset: 0x000CA1D0
	public static void Setup(RewiredInputs _component)
	{
		ControlsInput.m_availableRewiredPlayers = new List<int>(ControlsInput.m_startingRewiredPlayers);
		ControlsInput.m_assignedRewiredPlayers.Clear();
		ControlsInput.m_playerInputManager.Clear();
		ControlsInput.m_playerInputManager.Add(0, _component);
		ControlsInput.m_inputManager = _component;
		ControlsInput.m_inputManager.Initialize(0);
		ControlsInput.InitSpecialListenedAction(ref ControlsInput.m_inputManager);
	}

	// Token: 0x060022B2 RID: 8882 RVA: 0x000CC028 File Offset: 0x000CA228
	private static void InitSpecialListenedAction(ref RewiredInputs _manager)
	{
		for (int i = 0; i < ControlsInput.m_specialListenedActions.Length; i++)
		{
			_manager.AddSpecialListenedActions(ControlsInput.GetGameplayActionName(ControlsInput.m_specialListenedActions[i]));
		}
		for (int j = 0; j < ControlsInput.m_quickReleaseActions.Length; j++)
		{
			_manager.AddActiveTimeWatcher(ControlsInput.GetGameplayActionName(ControlsInput.m_quickReleaseActions[j]));
		}
	}

	// Token: 0x060022B3 RID: 8883 RVA: 0x000CC08C File Offset: 0x000CA28C
	public static void StartCapture(int _playerID, int _mapCategoryID, InputAction _rewiredAction, AxisRange _axisRange, ControllerActionElementMapPair _pair, int _specificJoystickID, UnityAction<bool> _callback = null)
	{
		ControlsInput.m_playerInputManager[_playerID].StartCapture(_mapCategoryID, _rewiredAction, _axisRange, _pair, _specificJoystickID, _callback);
	}

	// Token: 0x060022B4 RID: 8884 RVA: 0x000CC0A8 File Offset: 0x000CA2A8
	public static void RemoveActionElementMap(int _playerID, InputAction _action, ControllerActionElementMapPair _mapToRemove, UnityAction<bool> _callback = null)
	{
		ControlsInput.m_playerInputManager[_playerID].RemoveMap(_action, _mapToRemove, _callback);
	}

	// Token: 0x060022B5 RID: 8885 RVA: 0x000CC0C0 File Offset: 0x000CA2C0
	public static void ResetDefaultInputs(int _playerID, bool _gamepad)
	{
		if (_gamepad)
		{
			ControlsInput.m_playerInputManager[_playerID].ResetGamepadToDefault();
		}
		else
		{
			ControlsInput.m_playerInputManager[_playerID].ResetKeyboardToDefault();
		}
	}

	// Token: 0x060022B6 RID: 8886 RVA: 0x000CC0F0 File Offset: 0x000CA2F0
	public static void CancelCapture(int _playerID)
	{
		ControlsInput.m_playerInputManager[_playerID].CancelCapture();
	}

	// Token: 0x060022B7 RID: 8887 RVA: 0x000CC104 File Offset: 0x000CA304
	public static int GetRewiredPlayerID()
	{
		int num = -1;
		if (ControlsInput.m_availableRewiredPlayers.Count > 0)
		{
			num = ControlsInput.m_availableRewiredPlayers[0];
			ControlsInput.m_availableRewiredPlayers.RemoveAt(0);
			ControlsInput.m_assignedRewiredPlayers.Add(num);
			if (!ControlsInput.m_playerInputManager.ContainsKey(num))
			{
				RewiredInputs rewiredInputs = UnityEngine.Object.Instantiate<RewiredInputs>(ControlsInput.m_inputManager);
				ControlsInput.m_playerInputManager.Add(num, rewiredInputs);
				rewiredInputs.transform.SetParent(ControlsInput.m_inputManager.transform.parent);
				rewiredInputs.Initialize(num);
				ControlsInput.InitSpecialListenedAction(ref rewiredInputs);
			}
			else
			{
				ControlsInput.m_playerInputManager[num].gameObject.SetActive(true);
			}
			if (!Global.ForceControllersP2 || ControlsInput.m_assignedRewiredPlayers.Count == 1)
			{
				ControlsInput.AssignControllerToPlayer(num);
			}
			else
			{
				ControlsInput.ForceControllerP2();
			}
			ControlsInput.ImportXmlData(num);
		}
		else
		{
			Debug.LogError("There is no Rewired Player available");
		}
		return num;
	}

	// Token: 0x060022B8 RID: 8888 RVA: 0x000CC1F0 File Offset: 0x000CA3F0
	public static void ForceControllerP2()
	{
		Controller[] controllers = ReInput.controllers.GetControllers(ControllerType.Joystick);
		foreach (Controller controller in controllers)
		{
			ControlsInput.m_playerInputManager[0].RemoveJoystick(controller.id);
			ControlsInput.m_playerInputManager[1].AssignJoystick(controller.id);
		}
	}

	// Token: 0x060022B9 RID: 8889 RVA: 0x000CC250 File Offset: 0x000CA450
	public static void ReleaseRewiredPlayerID(int _id)
	{
		if (ControlsInput.m_availableRewiredPlayers.Contains(_id))
		{
			Debug.LogError(_id.ToString() + " was already in the available Rewired ID, either two players had the same id, or it was not remove from the avaialble id when asked for an id");
			return;
		}
		if (ControlsInput.m_playerInputManager[_id])
		{
			ControlsInput.m_playerInputManager[_id].gameObject.SetActive(false);
		}
		ControlsInput.m_assignedRewiredPlayers.Remove(_id);
		ControlsInput.m_assignedRewiredPlayers.Sort();
		ControlsInput.m_availableRewiredPlayers.Add(_id);
		ControlsInput.m_availableRewiredPlayers.Sort();
		ControlsInput.ReleasePlayersControllers(_id);
	}

	// Token: 0x060022BA RID: 8890 RVA: 0x000CC2E8 File Offset: 0x000CA4E8
	public static void ImportXmlData(int _playerID)
	{
		ControlsInput.m_playerInputManager[_playerID].ImportXmlData();
	}

	// Token: 0x060022BB RID: 8891 RVA: 0x000CC2FC File Offset: 0x000CA4FC
	public static void ExportXmlData(int _playerID)
	{
		ControlsInput.m_playerInputManager[_playerID].ExportXmlData();
	}

	// Token: 0x060022BC RID: 8892 RVA: 0x000CC310 File Offset: 0x000CA510
	public static void SetActiveMapCategory(int _playerID, ControlsInput.MapCategories _category)
	{
		if (_category == ControlsInput.MapCategories.Gameplay)
		{
			ControlsInput.m_playerInputManager[_playerID].EnableMapping("Default", true);
		}
		else if (_category == ControlsInput.MapCategories.Menu)
		{
			ControlsInput.m_playerInputManager[_playerID].EnableMapping("Default", false);
		}
	}

	// Token: 0x060022BD RID: 8893 RVA: 0x000CC350 File Offset: 0x000CA550
	public static void SetMovementActive(int _playerID, bool _active)
	{
		ControlsInput.m_playerInputManager[_playerID].EnableMapping("Movement", _active);
	}

	// Token: 0x060022BE RID: 8894 RVA: 0x000CC368 File Offset: 0x000CA568
	public static void SetMenuActive(int _playerID, bool _active)
	{
		ControlsInput.m_playerInputManager[_playerID].EnableMapping("Menu", _active);
	}

	// Token: 0x060022BF RID: 8895 RVA: 0x000CC380 File Offset: 0x000CA580
	public static void SetCursorActive(int _playerID, bool _active)
	{
		ControlsInput.m_playerInputManager[_playerID].EnableMapping("VirtualCursor", _active);
	}

	// Token: 0x060022C0 RID: 8896 RVA: 0x000CC398 File Offset: 0x000CA598
	public static void SetActionActive(int _playerID, bool _active)
	{
		ControlsInput.m_playerInputManager[_playerID].EnableMapping("Actions", _active);
	}

	// Token: 0x060022C1 RID: 8897 RVA: 0x000CC3B0 File Offset: 0x000CA5B0
	public static void SetCameraActive(int _playerID, bool _active)
	{
		ControlsInput.m_playerInputManager[_playerID].EnableMapping("Camera", _active);
	}

	// Token: 0x060022C2 RID: 8898 RVA: 0x000CC3C8 File Offset: 0x000CA5C8
	public static void SetDeployActive(int _playerID, bool _active)
	{
		ControlsInput.m_playerInputManager[_playerID].EnableMapping("Deploy", _active);
	}

	// Token: 0x060022C3 RID: 8899 RVA: 0x000CC3E0 File Offset: 0x000CA5E0
	public static void SetQuickSlotActive(int _playerID, bool _active)
	{
		ControlsInput.m_playerInputManager[_playerID].EnableMapping("QuickSlot", _active);
	}

	// Token: 0x060022C4 RID: 8900 RVA: 0x000CC3F8 File Offset: 0x000CA5F8
	public static void AssignControllerToPlayer(int _playerID)
	{
		if (ControlsInput.m_assignedRewiredPlayers.Count == 1)
		{
			ControlsInput.m_playerInputManager[0].EnableMouseControl();
			foreach (Joystick joystick in ReInput.controllers.Joysticks)
			{
				ControlsInput.m_playerInputManager[0].AssignJoystick(joystick.id);
			}
		}
		else
		{
			Controller[] array = new Controller[ControlsInput.m_assignedRewiredPlayers.Count];
			for (int i = 0; i < ControlsInput.m_playerInputManager.Count; i++)
			{
				if (ControlsInput.m_playerInputManager[i].PlayerID != _playerID && ReInput.controllers.joystickCount > 1)
				{
					array[i] = ControlsInput.m_playerInputManager[i].GetLastActiveJoystick();
				}
				ControlsInput.m_playerInputManager[i].ClearAllControllers();
				if (array[i] != null)
				{
					ControlsInput.m_playerInputManager[i].AssignJoystick(array[i].id);
				}
			}
			foreach (Joystick joystick2 in ReInput.controllers.Joysticks)
			{
				if (!array.Contains(joystick2))
				{
					ControlsInput.AssignControllerToNextPlayer(joystick2, _playerID);
					_playerID = -1;
				}
			}
		}
		ControlsInput.m_playerInputManager[0].EnableMouseControl();
		for (int j = 0; j < ControlsInput.m_assignedRewiredPlayers.Count; j++)
		{
			string text = "Player " + j.ToString() + "'s Joysticks\n";
			Player player = ReInput.players.GetPlayer(ControlsInput.m_assignedRewiredPlayers[j]);
			for (int k = 0; k < player.controllers.joystickCount; k++)
			{
				Joystick joystick3 = player.controllers.Joysticks[k];
				string text2 = text;
				text = string.Concat(new object[]
				{
					text2,
					joystick3.hardwareName,
					" ",
					joystick3.id,
					" ",
					joystick3.hardwareIdentifier,
					"\n"
				});
			}
			Debug.Log(text);
		}
	}

	// Token: 0x060022C5 RID: 8901 RVA: 0x000CC674 File Offset: 0x000CA874
	public static int AssignControllerToNextPlayer(Joystick _joystick, int _forceAssignToPlayer = -1)
	{
		int num = -1;
		if (_forceAssignToPlayer == -1)
		{
			int num2 = 0;
			if (ReInput.controllers.joystickCount < ControlsInput.m_assignedRewiredPlayers.Count)
			{
				num2 = 1;
			}
			int num3 = -1;
			for (int i = num2; i < ControlsInput.m_playerInputManager.Count; i++)
			{
				int assignedJoystickCount = ControlsInput.m_playerInputManager[i].GetAssignedJoystickCount();
				if (num == -1 || assignedJoystickCount < num3)
				{
					num = i;
					num3 = assignedJoystickCount;
				}
			}
		}
		else
		{
			num = _forceAssignToPlayer;
		}
		if (num != -1)
		{
			ControlsInput.m_playerInputManager[num].AssignJoystick(_joystick.id);
		}
		return num;
	}

	// Token: 0x060022C6 RID: 8902 RVA: 0x000CC710 File Offset: 0x000CA910
	public static void ReleasePlayersControllers(int _playerID)
	{
		Player player = ReInput.players.GetPlayer(_playerID);
		List<Joystick> list = new List<Joystick>(player.controllers.Joysticks);
		foreach (Joystick joystick in list)
		{
			ControlsInput.AssignControllerToNextPlayer(joystick, -1);
		}
		ControlsInput.m_playerInputManager[_playerID].ClearAllControllers();
	}

	// Token: 0x060022C7 RID: 8903 RVA: 0x000CC798 File Offset: 0x000CA998
	public static void RefreshControllerAssignation()
	{
		for (int i = 0; i < ControlsInput.m_assignedRewiredPlayers.Count; i++)
		{
			int key = ControlsInput.m_assignedRewiredPlayers[i];
			ControlsInput.m_playerInputManager[key].ClearAllControllers();
		}
		int num = 0;
		int num2 = 0;
		if (ControlsInput.m_assignedRewiredPlayers.Count > 0 && ReInput.controllers != null)
		{
			ControlsInput.m_playerInputManager[0].EnableMouseControl();
			for (int j = 1; j < ControlsInput.m_playerInputManager.Count; j++)
			{
				ControlsInput.m_playerInputManager[j].DisableMouseControl();
			}
			foreach (Joystick joystick in ReInput.controllers.Joysticks)
			{
				if (ReInput.controllers.Joysticks.Count >= ControlsInput.m_assignedRewiredPlayers.Count)
				{
					if (ControlsInput.m_assignedRewiredPlayers.Count > 1 && num != 0)
					{
						num2++;
					}
				}
				else if (num == 0)
				{
					num2 = 1;
				}
				else
				{
					num2++;
				}
				if (num2 >= ControlsInput.m_assignedRewiredPlayers.Count)
				{
					num2 = 0;
				}
				int key2 = ControlsInput.m_assignedRewiredPlayers[num2];
				ControlsInput.m_playerInputManager[key2].AssignJoystick(joystick.id);
				num++;
			}
			if (ControlsInput.m_assignedRewiredPlayers.Count - 1 > ReInput.controllers.Joysticks.Count)
			{
				Debug.LogWarning("Not every player has a controller assigned");
			}
		}
	}

	// Token: 0x060022C8 RID: 8904 RVA: 0x000CC93C File Offset: 0x000CAB3C
	public static void AssignMouseKeyboardToPlayer(int _playerID)
	{
		for (int i = 0; i < ControlsInput.m_playerInputManager.Count; i++)
		{
			if (i == _playerID)
			{
				ControlsInput.m_playerInputManager[_playerID].EnableMouseControl();
			}
			else
			{
				ControlsInput.m_playerInputManager[_playerID].DisableMouseControl();
			}
		}
	}

	// Token: 0x060022C9 RID: 8905 RVA: 0x000CC990 File Offset: 0x000CAB90
	public static void DisableKeyboardControl(int _playerID)
	{
		ControlsInput.m_playerInputManager[_playerID].DisableMouseControl();
	}

	// Token: 0x060022CA RID: 8906 RVA: 0x000CC9A4 File Offset: 0x000CABA4
	public static void EnableKeyboardControl(int _playerID)
	{
		ControlsInput.m_playerInputManager[_playerID].EnableMouseControl();
	}

	// Token: 0x060022CB RID: 8907 RVA: 0x000CC9B8 File Offset: 0x000CABB8
	public static bool CheckEnoughControllers()
	{
		return ReInput.controllers.joystickCount > 0;
	}

	// Token: 0x060022CC RID: 8908 RVA: 0x000CC9C8 File Offset: 0x000CABC8
	public static void SetController(int _controllerID, int _playerID)
	{
		for (int i = 0; i < ControlsInput.m_playerInputManager.Count; i++)
		{
			if (i == _playerID)
			{
				ControlsInput.m_playerInputManager[i].AssignJoystick(_controllerID);
			}
			else
			{
				ControlsInput.m_playerInputManager[i].RemoveJoystick(_controllerID);
			}
		}
	}

	// Token: 0x060022CD RID: 8909 RVA: 0x000CCA20 File Offset: 0x000CAC20
	private void JoystickConnected(ControllerStatusChangedEventArgs args)
	{
		Debug.Log("bleh connected");
	}

	// Token: 0x060022CE RID: 8910 RVA: 0x000CCA2C File Offset: 0x000CAC2C
	private void JoystickPreDisconnect(ControllerStatusChangedEventArgs args)
	{
	}

	// Token: 0x060022CF RID: 8911 RVA: 0x000CCA30 File Offset: 0x000CAC30
	private void JoystickDisconnected(ControllerStatusChangedEventArgs args)
	{
		Debug.Log("bleh disconnected");
	}

	// Token: 0x060022D0 RID: 8912 RVA: 0x000CCA3C File Offset: 0x000CAC3C
	public static bool AnyInputDown(int _playerID)
	{
		return ControlsInput.m_playerInputManager[_playerID].GetAnyButtonDown();
	}

	// Token: 0x060022D1 RID: 8913 RVA: 0x000CCA50 File Offset: 0x000CAC50
	public static bool MenuCompare(int _playerID)
	{
		return ControlsInput.m_playerInputManager[_playerID].GetButtonDown(ControlsInput.GetMenuActionName(ControlsInput.MenuActions.Compare));
	}

	// Token: 0x060022D2 RID: 8914 RVA: 0x000CCA6C File Offset: 0x000CAC6C
	public static float MenuVerticalAxis(int _playerID)
	{
		return ControlsInput.m_playerInputManager[_playerID].GetAxis(ControlsInput.GetMenuActionName(ControlsInput.MenuActions.MoveVertical));
	}

	// Token: 0x060022D3 RID: 8915 RVA: 0x000CCA84 File Offset: 0x000CAC84
	public static bool MenuDown(int _playerID)
	{
		return ControlsInput.m_playerInputManager[_playerID].GetButtonDown(ControlsInput.GetMenuActionName(ControlsInput.MenuActions.MoveDown));
	}

	// Token: 0x060022D4 RID: 8916 RVA: 0x000CCA9C File Offset: 0x000CAC9C
	public static bool MenuUp(int _playerID)
	{
		return ControlsInput.m_playerInputManager[_playerID].GetButtonDown(ControlsInput.GetMenuActionName(ControlsInput.MenuActions.MoveUp));
	}

	// Token: 0x060022D5 RID: 8917 RVA: 0x000CCAB4 File Offset: 0x000CACB4
	public static float MenuHorizontalAxis(int _playerID)
	{
		return ControlsInput.m_playerInputManager[_playerID].GetAxis(ControlsInput.GetMenuActionName(ControlsInput.MenuActions.MoveHorizontal));
	}

	// Token: 0x060022D6 RID: 8918 RVA: 0x000CCACC File Offset: 0x000CACCC
	public static bool MenuLeft(int _playerID)
	{
		return ControlsInput.m_playerInputManager[_playerID].GetButtonDown(ControlsInput.GetMenuActionName(ControlsInput.MenuActions.MoveLeft));
	}

	// Token: 0x060022D7 RID: 8919 RVA: 0x000CCAE4 File Offset: 0x000CACE4
	public static bool MenuRight(int _playerID)
	{
		return ControlsInput.m_playerInputManager[_playerID].GetButtonDown(ControlsInput.GetMenuActionName(ControlsInput.MenuActions.MoveRight));
	}

	// Token: 0x060022D8 RID: 8920 RVA: 0x000CCAFC File Offset: 0x000CACFC
	public static bool MenuLeftHold(int _playerID)
	{
		return ControlsInput.m_playerInputManager[_playerID].GetButton(ControlsInput.GetMenuActionName(ControlsInput.MenuActions.MoveLeft));
	}

	// Token: 0x060022D9 RID: 8921 RVA: 0x000CCB14 File Offset: 0x000CAD14
	public static bool MenuRightHold(int _playerID)
	{
		return ControlsInput.m_playerInputManager[_playerID].GetButton(ControlsInput.GetMenuActionName(ControlsInput.MenuActions.MoveRight));
	}

	// Token: 0x060022DA RID: 8922 RVA: 0x000CCB2C File Offset: 0x000CAD2C
	public static bool ToggleHelp(int _playerID)
	{
		return !ControlsInput.QuickSlotToggled(_playerID) && ControlsInput.m_playerInputManager[_playerID].GetButtonDown(ControlsInput.GetMenuActionName(ControlsInput.MenuActions.ToggleHelp));
	}

	// Token: 0x060022DB RID: 8923 RVA: 0x000CCB54 File Offset: 0x000CAD54
	public static bool MenuQuickAction(int _playerID)
	{
		return !ControlsInput.QuickSlotToggled(_playerID) && ControlsInput.m_playerInputManager[_playerID].GetButtonDown(ControlsInput.GetMenuActionName(ControlsInput.MenuActions.QuickAction));
	}

	// Token: 0x060022DC RID: 8924 RVA: 0x000CCB7C File Offset: 0x000CAD7C
	public static bool MenuShowDetails(int _playerID)
	{
		return !ControlsInput.QuickSlotToggled(_playerID) && ControlsInput.m_playerInputManager[_playerID].GetButtonDown(ControlsInput.GetMenuActionName(ControlsInput.MenuActions.ShowOptions));
	}

	// Token: 0x060022DD RID: 8925 RVA: 0x000CCBA4 File Offset: 0x000CADA4
	public static bool MenuCancel(int _playerID)
	{
		return !ControlsInput.QuickSlotToggled(_playerID) && ControlsInput.m_playerInputManager[_playerID].GetButtonDown(ControlsInput.GetMenuActionName(ControlsInput.MenuActions.Cancel));
	}

	// Token: 0x060022DE RID: 8926 RVA: 0x000CCBCC File Offset: 0x000CADCC
	public static bool ExitContainer(int _playerID)
	{
		return !ControlsInput.QuickSlotToggled(_playerID) && ControlsInput.m_playerInputManager[_playerID].GetButtonDown(ControlsInput.GetMenuActionName(ControlsInput.MenuActions.ExitContainer));
	}

	// Token: 0x060022DF RID: 8927 RVA: 0x000CCBF4 File Offset: 0x000CADF4
	public static bool TakeAll(int _playerID)
	{
		return !ControlsInput.QuickSlotToggled(_playerID) && ControlsInput.m_playerInputManager[_playerID].GetButtonDown(ControlsInput.GetMenuActionName(ControlsInput.MenuActions.TakeAll));
	}

	// Token: 0x060022E0 RID: 8928 RVA: 0x000CCC1C File Offset: 0x000CAE1C
	public static bool InfoInput(int _playerID)
	{
		return !ControlsInput.QuickSlotToggled(_playerID) && ControlsInput.m_playerInputManager[_playerID].GetButtonDown(ControlsInput.GetMenuActionName(ControlsInput.MenuActions.InfoInput));
	}

	// Token: 0x060022E1 RID: 8929 RVA: 0x000CCC44 File Offset: 0x000CAE44
	public static bool MenuCancelSystem()
	{
		return ReInput.players.GetPlayer("SYSTEM").GetButtonDown(ControlsInput.GetMenuActionName(ControlsInput.MenuActions.Cancel));
	}

	// Token: 0x060022E2 RID: 8930 RVA: 0x000CCC60 File Offset: 0x000CAE60
	public static bool ToggleInventory(int _playerID)
	{
		return ControlsInput.m_playerInputManager[_playerID].GetButtonDown(ControlsInput.GetMenuActionName(ControlsInput.MenuActions.ToggleInventory));
	}

	// Token: 0x060022E3 RID: 8931 RVA: 0x000CCC7C File Offset: 0x000CAE7C
	public static bool ToggleChatMenu(int _playerID)
	{
		return ControlsInput.m_playerInputManager[_playerID].GetButtonDown(ControlsInput.GetMenuActionName(ControlsInput.MenuActions.ToggleChatMenu));
	}

	// Token: 0x060022E4 RID: 8932 RVA: 0x000CCC98 File Offset: 0x000CAE98
	public static bool ToggleEquipment(int _playerID)
	{
		return ControlsInput.m_playerInputManager[_playerID].GetButtonDown(ControlsInput.GetMenuActionName(ControlsInput.MenuActions.ToggleEquipment));
	}

	// Token: 0x060022E5 RID: 8933 RVA: 0x000CCCB4 File Offset: 0x000CAEB4
	public static bool ToggleQuestLog(int _playerID)
	{
		return ControlsInput.m_playerInputManager[_playerID].GetButtonDown(ControlsInput.GetMenuActionName(ControlsInput.MenuActions.ToggleQuestMenu));
	}

	// Token: 0x060022E6 RID: 8934 RVA: 0x000CCCD0 File Offset: 0x000CAED0
	public static bool ToggleMap(int _playerID)
	{
		return ControlsInput.m_playerInputManager[_playerID].GetButtonDown(ControlsInput.GetMenuActionName(ControlsInput.MenuActions.ToggleMapMenu));
	}

	// Token: 0x060022E7 RID: 8935 RVA: 0x000CCCEC File Offset: 0x000CAEEC
	public static bool ToggleSkillMenu(int _playerID)
	{
		return ControlsInput.m_playerInputManager[_playerID].GetButtonDown(ControlsInput.GetMenuActionName(ControlsInput.MenuActions.ToggleSkillMenu));
	}

	// Token: 0x060022E8 RID: 8936 RVA: 0x000CCD08 File Offset: 0x000CAF08
	public static bool ToggleCharacterStatusMenu(int _playerID)
	{
		return ControlsInput.m_playerInputManager[_playerID].GetButtonDown(ControlsInput.GetMenuActionName(ControlsInput.MenuActions.ToggleCharacterStatus));
	}

	// Token: 0x060022E9 RID: 8937 RVA: 0x000CCD24 File Offset: 0x000CAF24
	public static bool ToggleEffectMenu(int _playerID)
	{
		return ControlsInput.m_playerInputManager[_playerID].GetButtonDown(ControlsInput.GetMenuActionName(ControlsInput.MenuActions.ToggleEffectMenu));
	}

	// Token: 0x060022EA RID: 8938 RVA: 0x000CCD40 File Offset: 0x000CAF40
	public static bool ToggleCraftingMenu(int _playerID)
	{
		return ControlsInput.m_playerInputManager[_playerID].GetButtonDown(ControlsInput.GetMenuActionName(ControlsInput.MenuActions.ToggleCraftingMenu));
	}

	// Token: 0x060022EB RID: 8939 RVA: 0x000CCD5C File Offset: 0x000CAF5C
	public static bool ToggleQuickSlotMenu(int _playerID)
	{
		return ControlsInput.m_playerInputManager[_playerID].GetButtonDown(ControlsInput.GetMenuActionName(ControlsInput.MenuActions.ToggleQuickSlotMenu));
	}

	// Token: 0x060022EC RID: 8940 RVA: 0x000CCD78 File Offset: 0x000CAF78
	public static bool GoToPreviousMenu(int _playerID)
	{
		return ControlsInput.m_playerInputManager[_playerID].GetButtonDown(ControlsInput.GetMenuActionName(ControlsInput.MenuActions.GoToPreviousMenu));
	}

	// Token: 0x060022ED RID: 8941 RVA: 0x000CCD94 File Offset: 0x000CAF94
	public static bool GoToNextMenu(int _playerID)
	{
		return ControlsInput.m_playerInputManager[_playerID].GetButtonDown(ControlsInput.GetMenuActionName(ControlsInput.MenuActions.GoToNextMenu));
	}

	// Token: 0x060022EE RID: 8942 RVA: 0x000CCDB0 File Offset: 0x000CAFB0
	public static bool GoToPreviousFilterTab(int _playerID)
	{
		return ControlsInput.m_playerInputManager[_playerID].GetButtonDown(ControlsInput.GetMenuActionName(ControlsInput.MenuActions.PreviousFilter));
	}

	// Token: 0x060022EF RID: 8943 RVA: 0x000CCDCC File Offset: 0x000CAFCC
	public static bool GoToNextFilterTab(int _playerID)
	{
		return ControlsInput.m_playerInputManager[_playerID].GetButtonDown(ControlsInput.GetMenuActionName(ControlsInput.MenuActions.NextFilter));
	}

	// Token: 0x060022F0 RID: 8944 RVA: 0x000CCDE8 File Offset: 0x000CAFE8
	public static bool QuickDialogueUp(int _playerID)
	{
		return ControlsInput.m_playerInputManager[_playerID].GetButtonUp(ControlsInput.GetMenuActionName(ControlsInput.MenuActions.QuickDialogue));
	}

	// Token: 0x060022F1 RID: 8945 RVA: 0x000CCE04 File Offset: 0x000CB004
	public static bool GamepadSaveSelectionCheat(int _playerID)
	{
		for (int i = 1; i <= 5; i++)
		{
			if (!ControlsInput.m_playerInputManager[_playerID].GetButton(string.Format("EnableSaveSelection{0}", i)))
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x060022F2 RID: 8946 RVA: 0x000CCE4C File Offset: 0x000CB04C
	public static bool GamepadUnstuckCheat(int _playerID)
	{
		for (int i = 1; i <= 4; i++)
		{
			if (!ControlsInput.m_playerInputManager[_playerID].GetButton(string.Format("EnableSaveSelection{0}", i)))
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x060022F3 RID: 8947 RVA: 0x000CCE94 File Offset: 0x000CB094
	public static float MoveHorizontal(int _playerID)
	{
		return ControlsInput.m_playerInputManager[_playerID].GetAxis(ControlsInput.GetGameplayActionName(ControlsInput.GameplayActions.MoveHorizontal));
	}

	// Token: 0x060022F4 RID: 8948 RVA: 0x000CCEAC File Offset: 0x000CB0AC
	public static float MoveVertical(int _playerID)
	{
		return ControlsInput.m_playerInputManager[_playerID].GetAxis(ControlsInput.GetGameplayActionName(ControlsInput.GameplayActions.MoveVertical));
	}

	// Token: 0x060022F5 RID: 8949 RVA: 0x000CCEC4 File Offset: 0x000CB0C4
	public static float RotateCameraHorizontal(int _playerID)
	{
		return ControlsInput.m_playerInputManager[_playerID].GetAxis(ControlsInput.GetGameplayActionName(ControlsInput.GameplayActions.RotateCameraHorizontal));
	}

	// Token: 0x060022F6 RID: 8950 RVA: 0x000CCEDC File Offset: 0x000CB0DC
	public static float RotateCameraVertical(int _playerID)
	{
		return ControlsInput.m_playerInputManager[_playerID].GetAxis(ControlsInput.GetGameplayActionName(ControlsInput.GameplayActions.RotateCameraVertical));
	}

	// Token: 0x060022F7 RID: 8951 RVA: 0x000CCEF4 File Offset: 0x000CB0F4
	public static bool Sprint(int _playerID)
	{
		return ControlsInput.m_playerInputManager[_playerID].GetButton(ControlsInput.GetGameplayActionName(ControlsInput.GameplayActions.Sprint));
	}

	// Token: 0x060022F8 RID: 8952 RVA: 0x000CCF10 File Offset: 0x000CB110
	public static bool DodgeButtonDown(int _playerID)
	{
		return ControlsInput.m_playerInputManager[_playerID].GetButtonDown(ControlsInput.GetGameplayActionName(ControlsInput.GameplayActions.Dodge));
	}

	// Token: 0x060022F9 RID: 8953 RVA: 0x000CCF2C File Offset: 0x000CB12C
	public static bool DodgeButton(int _playerID)
	{
		if (ControlsInput.m_playerInputManager[_playerID].GetLastActiveController() != null && ControlsInput.m_playerInputManager[_playerID].GetLastActiveController().type == ControllerType.Joystick)
		{
			return ControlsInput.m_playerInputManager[_playerID].GetButtonUpQuick(ControlsInput.GetGameplayActionName(ControlsInput.GameplayActions.Dodge), 0.18f);
		}
		return ControlsInput.m_playerInputManager[_playerID].GetButtonDown(ControlsInput.GetGameplayActionName(ControlsInput.GameplayActions.Dodge));
	}

	// Token: 0x060022FA RID: 8954 RVA: 0x000CCFA0 File Offset: 0x000CB1A0
	public static bool StealthButton(int _playerID)
	{
		if (ControlsInput.m_playerInputManager[_playerID].GetLastActiveController() != null && ControlsInput.m_playerInputManager[_playerID].GetLastActiveController().type == ControllerType.Joystick)
		{
			return ControlsInput.m_playerInputManager[_playerID].GetButtonUpQuick(ControlsInput.GetGameplayActionName(ControlsInput.GameplayActions.Stealth), 0.25f);
		}
		return ControlsInput.m_playerInputManager[_playerID].GetButtonDown(ControlsInput.GetGameplayActionName(ControlsInput.GameplayActions.Stealth));
	}

	// Token: 0x060022FB RID: 8955 RVA: 0x000CD014 File Offset: 0x000CB214
	public static bool DragCorpse(int _playerID)
	{
		return ControlsInput.m_playerInputManager[_playerID].GetButton(ControlsInput.GetGameplayActionName(ControlsInput.GameplayActions.DragCorpse));
	}

	// Token: 0x060022FC RID: 8956 RVA: 0x000CD030 File Offset: 0x000CB230
	public static bool DragCorpseUp(int _playerID)
	{
		return ControlsInput.m_playerInputManager[_playerID].GetButtonUp(ControlsInput.GetGameplayActionName(ControlsInput.GameplayActions.DragCorpse));
	}

	// Token: 0x060022FD RID: 8957 RVA: 0x000CD04C File Offset: 0x000CB24C
	public static bool HandleBackpack(int _playerID)
	{
		return ControlsInput.m_playerInputManager[_playerID].GetButtonUp(ControlsInput.GetGameplayActionName(ControlsInput.GameplayActions.HandleBag));
	}

	// Token: 0x060022FE RID: 8958 RVA: 0x000CD068 File Offset: 0x000CB268
	public static bool AutoRun(int _playerID)
	{
		return ControlsInput.m_playerInputManager[_playerID].GetButtonDown(ControlsInput.GetGameplayActionName(ControlsInput.GameplayActions.AutoRun));
	}

	// Token: 0x060022FF RID: 8959 RVA: 0x000CD084 File Offset: 0x000CB284
	public static bool LockHoldDown(int _playerID)
	{
		return ControlsInput.m_playerInputManager[_playerID].GetButtonDown(ControlsInput.GetGameplayActionName(ControlsInput.GameplayActions.LockHold));
	}

	// Token: 0x06002300 RID: 8960 RVA: 0x000CD0A0 File Offset: 0x000CB2A0
	public static bool LockHoldUp(int _playerID)
	{
		return ControlsInput.m_playerInputManager[_playerID].GetButtonUp(ControlsInput.GetGameplayActionName(ControlsInput.GameplayActions.LockHold));
	}

	// Token: 0x06002301 RID: 8961 RVA: 0x000CD0BC File Offset: 0x000CB2BC
	public static bool LockToggle(int _playerID)
	{
		return ControlsInput.m_playerInputManager[_playerID].GetButtonDown(ControlsInput.GetGameplayActionName(ControlsInput.GameplayActions.LockToggle));
	}

	// Token: 0x06002302 RID: 8962 RVA: 0x000CD0D8 File Offset: 0x000CB2D8
	public static float SwitchTargetHorizontal(int _playerID)
	{
		return ControlsInput.m_playerInputManager[_playerID].GetAxis(ControlsInput.GetGameplayActionName(ControlsInput.GameplayActions.SwitchTargetHorizontal));
	}

	// Token: 0x06002303 RID: 8963 RVA: 0x000CD0F4 File Offset: 0x000CB2F4
	public static float SwitchTargetVertical(int _playerID)
	{
		return ControlsInput.m_playerInputManager[_playerID].GetAxis(ControlsInput.GetGameplayActionName(ControlsInput.GameplayActions.SwitchTargetVertical));
	}

	// Token: 0x06002304 RID: 8964 RVA: 0x000CD110 File Offset: 0x000CB310
	public static bool Sheathe(int _playerID)
	{
		return ControlsInput.m_playerInputManager[_playerID].GetButtonDown(ControlsInput.GetGameplayActionName(ControlsInput.GameplayActions.Sheathe));
	}

	// Token: 0x06002305 RID: 8965 RVA: 0x000CD12C File Offset: 0x000CB32C
	public static bool Attack1(int _playerID)
	{
		return ControlsInput.m_playerInputManager[_playerID].GetButtonDown(ControlsInput.GetGameplayActionName(ControlsInput.GameplayActions.Attack1));
	}

	// Token: 0x06002306 RID: 8966 RVA: 0x000CD144 File Offset: 0x000CB344
	public static bool Attack1Press(int _playerID)
	{
		return ControlsInput.m_playerInputManager[_playerID].GetButton(ControlsInput.GetGameplayActionName(ControlsInput.GameplayActions.Attack1));
	}

	// Token: 0x06002307 RID: 8967 RVA: 0x000CD15C File Offset: 0x000CB35C
	public static bool Attack2(int _playerID)
	{
		return ControlsInput.m_playerInputManager[_playerID].GetButtonDown(ControlsInput.GetGameplayActionName(ControlsInput.GameplayActions.Attack2));
	}

	// Token: 0x06002308 RID: 8968 RVA: 0x000CD174 File Offset: 0x000CB374
	public static bool Attack1Release(int _playerID)
	{
		return ControlsInput.m_playerInputManager[_playerID].GetButtonUp(ControlsInput.GetGameplayActionName(ControlsInput.GameplayActions.Attack1));
	}

	// Token: 0x06002309 RID: 8969 RVA: 0x000CD18C File Offset: 0x000CB38C
	public static bool Attack2Release(int _playerID)
	{
		return ControlsInput.m_playerInputManager[_playerID].GetButtonUp(ControlsInput.GetGameplayActionName(ControlsInput.GameplayActions.Attack2));
	}

	// Token: 0x0600230A RID: 8970 RVA: 0x000CD1A4 File Offset: 0x000CB3A4
	public static bool AttackWhenZoomedPress(int _playerID)
	{
		return ControlsInput.m_playerInputManager[_playerID].GetButton(ControlsInput.GetGameplayActionName(ControlsInput.GameplayActions.ChargeWeapon));
	}

	// Token: 0x0600230B RID: 8971 RVA: 0x000CD1C0 File Offset: 0x000CB3C0
	public static bool AttackWhenZoomed(int _playerID)
	{
		return ControlsInput.m_playerInputManager[_playerID].GetButtonDown(ControlsInput.GetGameplayActionName(ControlsInput.GameplayActions.ChargeWeapon));
	}

	// Token: 0x0600230C RID: 8972 RVA: 0x000CD1DC File Offset: 0x000CB3DC
	public static bool AttackWhenZoomedRelease(int _playerID)
	{
		return ControlsInput.m_playerInputManager[_playerID].GetButtonUp(ControlsInput.GetGameplayActionName(ControlsInput.GameplayActions.ChargeWeapon));
	}

	// Token: 0x0600230D RID: 8973 RVA: 0x000CD1F8 File Offset: 0x000CB3F8
	public static bool CancelChargeWeapon(int _playerID)
	{
		return ControlsInput.m_playerInputManager[_playerID].GetButtonUp(ControlsInput.GetGameplayActionName(ControlsInput.GameplayActions.CancelCharge));
	}

	// Token: 0x0600230E RID: 8974 RVA: 0x000CD214 File Offset: 0x000CB414
	public static bool Block(int _playerID)
	{
		return ControlsInput.m_playerInputManager[_playerID].GetButton(ControlsInput.GetGameplayActionName(ControlsInput.GameplayActions.Block));
	}

	// Token: 0x0600230F RID: 8975 RVA: 0x000CD22C File Offset: 0x000CB42C
	public static bool BlockRelease(int _playerID)
	{
		return ControlsInput.m_playerInputManager[_playerID].GetButtonUp(ControlsInput.GetGameplayActionName(ControlsInput.GameplayActions.Block));
	}

	// Token: 0x06002310 RID: 8976 RVA: 0x000CD244 File Offset: 0x000CB444
	public static bool Aim(int _playerID)
	{
		return ControlsInput.m_playerInputManager[_playerID].GetButtonDown(ControlsInput.GetGameplayActionName(ControlsInput.GameplayActions.Aim));
	}

	// Token: 0x06002311 RID: 8977 RVA: 0x000CD260 File Offset: 0x000CB460
	public static bool Interact(int _playerID)
	{
		return ControlsInput.m_playerInputManager[_playerID].GetButtonDown(ControlsInput.GetGameplayActionName(ControlsInput.GameplayActions.Interact));
	}

	// Token: 0x06002312 RID: 8978 RVA: 0x000CD278 File Offset: 0x000CB478
	public static bool InteractHold(int _playerID)
	{
		return ControlsInput.m_playerInputManager[_playerID].GetButton(ControlsInput.GetGameplayActionName(ControlsInput.GameplayActions.Interact));
	}

	// Token: 0x06002313 RID: 8979 RVA: 0x000CD290 File Offset: 0x000CB490
	public static bool InteractUp(int _playerID)
	{
		return ControlsInput.m_playerInputManager[_playerID].GetButtonUp(ControlsInput.GetGameplayActionName(ControlsInput.GameplayActions.Interact));
	}

	// Token: 0x06002314 RID: 8980 RVA: 0x000CD2A8 File Offset: 0x000CB4A8
	public static bool ToggleLights(int _playerID)
	{
		return ControlsInput.m_playerInputManager[_playerID].GetButtonDown(ControlsInput.GetGameplayActionName(ControlsInput.GameplayActions.ToggleLights));
	}

	// Token: 0x06002315 RID: 8981 RVA: 0x000CD2C4 File Offset: 0x000CB4C4
	public static bool ConfirmDeploy(int _playerID)
	{
		return ControlsInput.m_playerInputManager[_playerID].GetButtonDown(ControlsInput.GetGameplayActionName(ControlsInput.GameplayActions.ConfirmDeploy));
	}

	// Token: 0x06002316 RID: 8982 RVA: 0x000CD2E0 File Offset: 0x000CB4E0
	public static bool CancelDeploy(int _playerID)
	{
		return ControlsInput.m_playerInputManager[_playerID].GetButtonDown(ControlsInput.GetGameplayActionName(ControlsInput.GameplayActions.CancelDeploy));
	}

	// Token: 0x06002317 RID: 8983 RVA: 0x000CD2FC File Offset: 0x000CB4FC
	public static bool QuickSlotToggled(int _playerID)
	{
		return ControlsInput.QuickSlotToggle1(_playerID) || ControlsInput.QuickSlotToggle2(_playerID);
	}

	// Token: 0x06002318 RID: 8984 RVA: 0x000CD314 File Offset: 0x000CB514
	public static bool QuickSlotToggle1(int _playerID)
	{
		return ControlsInput.m_playerInputManager[_playerID].GetButton("QuickSlotToggle1");
	}

	// Token: 0x06002319 RID: 8985 RVA: 0x000CD32C File Offset: 0x000CB52C
	public static bool QuickSlotToggle2(int _playerID)
	{
		return ControlsInput.m_playerInputManager[_playerID].GetButton("QuickSlotToggle2");
	}

	// Token: 0x0600231A RID: 8986 RVA: 0x000CD344 File Offset: 0x000CB544
	public static bool QuickSlot1(int _playerID)
	{
		return ControlsInput.m_playerInputManager[_playerID].GetButtonDown("QuickSlot1");
	}

	// Token: 0x0600231B RID: 8987 RVA: 0x000CD35C File Offset: 0x000CB55C
	public static bool QuickSlot2(int _playerID)
	{
		return ControlsInput.m_playerInputManager[_playerID].GetButtonDown("QuickSlot2");
	}

	// Token: 0x0600231C RID: 8988 RVA: 0x000CD374 File Offset: 0x000CB574
	public static bool QuickSlot3(int _playerID)
	{
		return ControlsInput.m_playerInputManager[_playerID].GetButtonDown("QuickSlot3");
	}

	// Token: 0x0600231D RID: 8989 RVA: 0x000CD38C File Offset: 0x000CB58C
	public static bool QuickSlot4(int _playerID)
	{
		return ControlsInput.m_playerInputManager[_playerID].GetButtonDown("QuickSlot4");
	}

	// Token: 0x0600231E RID: 8990 RVA: 0x000CD3A4 File Offset: 0x000CB5A4
	public static bool QuickSlotInstant1(int _playerID)
	{
		return ControlsInput.m_playerInputManager[_playerID].GetButtonDown("QS_Instant1") || (ControlsInput.QuickSlotToggle2(_playerID) && ControlsInput.QuickSlot1(_playerID));
	}

	// Token: 0x0600231F RID: 8991 RVA: 0x000CD3D8 File Offset: 0x000CB5D8
	public static bool QuickSlotInstant2(int _playerID)
	{
		return ControlsInput.m_playerInputManager[_playerID].GetButtonDown("QS_Instant2") || (ControlsInput.QuickSlotToggle2(_playerID) && ControlsInput.QuickSlot2(_playerID));
	}

	// Token: 0x06002320 RID: 8992 RVA: 0x000CD40C File Offset: 0x000CB60C
	public static bool QuickSlotInstant3(int _playerID)
	{
		return ControlsInput.m_playerInputManager[_playerID].GetButtonDown("QS_Instant3") || (ControlsInput.QuickSlotToggle2(_playerID) && ControlsInput.QuickSlot3(_playerID));
	}

	// Token: 0x06002321 RID: 8993 RVA: 0x000CD440 File Offset: 0x000CB640
	public static bool QuickSlotInstant4(int _playerID)
	{
		return ControlsInput.m_playerInputManager[_playerID].GetButtonDown("QS_Instant4") || (ControlsInput.QuickSlotToggle2(_playerID) && ControlsInput.QuickSlot4(_playerID));
	}

	// Token: 0x06002322 RID: 8994 RVA: 0x000CD474 File Offset: 0x000CB674
	public static bool QuickSlotInstant5(int _playerID)
	{
		return ControlsInput.m_playerInputManager[_playerID].GetButtonDown("QS_Instant5") || (ControlsInput.QuickSlotToggle1(_playerID) && ControlsInput.QuickSlot1(_playerID));
	}

	// Token: 0x06002323 RID: 8995 RVA: 0x000CD4A8 File Offset: 0x000CB6A8
	public static bool QuickSlotInstant6(int _playerID)
	{
		return ControlsInput.m_playerInputManager[_playerID].GetButtonDown("QS_Instant6") || (ControlsInput.QuickSlotToggle1(_playerID) && ControlsInput.QuickSlot2(_playerID));
	}

	// Token: 0x06002324 RID: 8996 RVA: 0x000CD4DC File Offset: 0x000CB6DC
	public static bool QuickSlotInstant7(int _playerID)
	{
		return ControlsInput.m_playerInputManager[_playerID].GetButtonDown("QS_Instant7") || (ControlsInput.QuickSlotToggle1(_playerID) && ControlsInput.QuickSlot3(_playerID));
	}

	// Token: 0x06002325 RID: 8997 RVA: 0x000CD510 File Offset: 0x000CB710
	public static bool QuickSlotInstant8(int _playerID)
	{
		return ControlsInput.m_playerInputManager[_playerID].GetButtonDown("QS_Instant8") || (ControlsInput.QuickSlotToggle1(_playerID) && ControlsInput.QuickSlot4(_playerID));
	}

	// Token: 0x06002326 RID: 8998 RVA: 0x000CD544 File Offset: 0x000CB744
	public static bool QuickSlotItem1(int _playerID)
	{
		return ControlsInput.m_playerInputManager[_playerID].GetButtonDown("QS_Item1");
	}

	// Token: 0x06002327 RID: 8999 RVA: 0x000CD55C File Offset: 0x000CB75C
	public static bool QuickSlotItem2(int _playerID)
	{
		return ControlsInput.m_playerInputManager[_playerID].GetButtonDown("QS_Item2");
	}

	// Token: 0x06002328 RID: 9000 RVA: 0x000CD574 File Offset: 0x000CB774
	public static bool QuickSlotItem3(int _playerID)
	{
		return ControlsInput.m_playerInputManager[_playerID].GetButtonDown("QS_Item3");
	}

	// Token: 0x06002329 RID: 9001 RVA: 0x000CD58C File Offset: 0x000CB78C
	public static bool GetVirtualCursorButtonDown(int _playerID, int _buttonID)
	{
		return !ControlsInput.QuickSlotToggled(_playerID) && ControlsInput.m_playerInputManager[_playerID].GetButtonDown(ControlsInput.GetVirtualCursorClickName(_buttonID));
	}

	// Token: 0x0600232A RID: 9002 RVA: 0x000CD5B4 File Offset: 0x000CB7B4
	public static bool GetVirtualCursorButtonUp(int _playerID, int _buttonID)
	{
		return !ControlsInput.QuickSlotToggled(_playerID) && ControlsInput.m_playerInputManager[_playerID].GetButtonUp(ControlsInput.GetVirtualCursorClickName(_buttonID));
	}

	// Token: 0x0600232B RID: 9003 RVA: 0x000CD5DC File Offset: 0x000CB7DC
	public static float GetVirtualCursorHorizontal(int _playerID)
	{
		return ControlsInput.m_playerInputManager[_playerID].GetAxis("VC_Horizontal");
	}

	// Token: 0x0600232C RID: 9004 RVA: 0x000CD5F4 File Offset: 0x000CB7F4
	public static float GetVirtualCursorVertical(int _playerID)
	{
		return ControlsInput.m_playerInputManager[_playerID].GetAxis("VC_Vertical");
	}

	// Token: 0x0600232D RID: 9005 RVA: 0x000CD60C File Offset: 0x000CB80C
	public static float GetVirtualCursorVerticalScroll(int _playerID)
	{
		return ControlsInput.m_playerInputManager[_playerID].GetAxis("VC_VerticalScroll");
	}

	// Token: 0x0600232E RID: 9006 RVA: 0x000CD624 File Offset: 0x000CB824
	public static float GetVirtualCursorHorizontalScroll(int _playerID)
	{
		return ControlsInput.m_playerInputManager[_playerID].GetAxis("VC_HorizontalScroll");
	}

	// Token: 0x0600232F RID: 9007 RVA: 0x000CD63C File Offset: 0x000CB83C
	public static bool GetActionDown(int _playerID, string _actionName)
	{
		return ControlsInput.m_playerInputManager[_playerID].GetButtonDown(_actionName);
	}

	// Token: 0x1700070C RID: 1804
	// (get) Token: 0x06002330 RID: 9008 RVA: 0x000CD650 File Offset: 0x000CB850
	public static int[] AssignedRewiredPlayer
	{
		get
		{
			return ControlsInput.m_assignedRewiredPlayers.ToArray();
		}
	}

	// Token: 0x06002331 RID: 9009 RVA: 0x000CD65C File Offset: 0x000CB85C
	public static float LastTimeMouseMouvement(int _playerID)
	{
		return ControlsInput.m_playerInputManager[_playerID].GetLastTimeMouseMovement();
	}

	// Token: 0x06002332 RID: 9010 RVA: 0x000CD670 File Offset: 0x000CB870
	public static float LastTimeVirtualCursorMovement(int _playerID)
	{
		float lastActionActiveTime = ControlsInput.m_playerInputManager[_playerID].GetLastActionActiveTime("VC_Horizontal");
		float lastActionActiveTime2 = ControlsInput.m_playerInputManager[_playerID].GetLastActionActiveTime("VC_Vertical");
		return Mathf.Max(lastActionActiveTime, lastActionActiveTime2);
	}

	// Token: 0x1700070D RID: 1805
	// (get) Token: 0x06002333 RID: 9011 RVA: 0x000CD6B0 File Offset: 0x000CB8B0
	public static Vector2 MousePosition
	{
		get
		{
			return ReInput.controllers.Mouse.screenPosition;
		}
	}

	// Token: 0x06002334 RID: 9012 RVA: 0x000CD6C4 File Offset: 0x000CB8C4
	public static bool DoesPlayerHasMouse(int _playerID)
	{
		return ReInput.players.GetPlayer(_playerID).controllers.hasMouse;
	}

	// Token: 0x06002335 RID: 9013 RVA: 0x000CD6DC File Offset: 0x000CB8DC
	public static bool DoesPlayerHasKeyboard(int _playerID)
	{
		return ControlsInput.m_playerInputManager[_playerID].GetIsKeyboardAssigned();
	}

	// Token: 0x06002336 RID: 9014 RVA: 0x000CD6F0 File Offset: 0x000CB8F0
	public static bool DoesPlayerHasGamepad(int _playerID)
	{
		return ControlsInput.m_playerInputManager[_playerID].GetAssignedJoystickCount() > 0;
	}

	// Token: 0x1700070E RID: 1806
	// (get) Token: 0x06002337 RID: 9015 RVA: 0x000CD708 File Offset: 0x000CB908
	public static bool IsInputReady
	{
		get
		{
			return ReInput.isReady;
		}
	}

	// Token: 0x06002338 RID: 9016 RVA: 0x000CD710 File Offset: 0x000CB910
	public static bool IsPlayerReady(int _playerID)
	{
		return ControlsInput.m_playerInputManager[_playerID] != null && ControlsInput.m_playerInputManager[_playerID].Initialized;
	}

	// Token: 0x06002339 RID: 9017 RVA: 0x000CD73C File Offset: 0x000CB93C
	public static bool IsCaptureActive(int _playerID)
	{
		return ControlsInput.m_playerInputManager[_playerID].IsCaptureActive;
	}

	// Token: 0x0600233A RID: 9018 RVA: 0x000CD750 File Offset: 0x000CB950
	public static string GetGameplayActionName(ControlsInput.GameplayActions _action)
	{
		return ControlsInput.REWIRED_GAMEPLAY_ACTION_NAMES[(int)_action];
	}

	// Token: 0x0600233B RID: 9019 RVA: 0x000CD75C File Offset: 0x000CB95C
	public static string GetMenuActionName(ControlsInput.MenuActions _action)
	{
		if (_action == ControlsInput.MenuActions.ShowOptions)
		{
			return ControlsInput.GetVirtualCursorClickName(1);
		}
		return ControlsInput.REWIRED_MENU_ACTION_NAMES[(int)_action];
	}

	// Token: 0x0600233C RID: 9020 RVA: 0x000CD774 File Offset: 0x000CB974
	public static string GetQuickSlotName(QuickSlot.QuickSlotIDs _slotID, bool _gamepad = false)
	{
		if (_slotID != QuickSlot.QuickSlotIDs.None)
		{
			string text = "QS_";
			if (!_gamepad)
			{
				switch (_slotID)
				{
				case QuickSlot.QuickSlotIDs.Item1:
				case QuickSlot.QuickSlotIDs.Item2:
				case QuickSlot.QuickSlotIDs.Item3:
					text += _slotID.ToString();
					break;
				default:
				{
					string str = text;
					string str2 = "Instant";
					int num = (int)_slotID;
					text = str + str2 + num.ToString();
					break;
				}
				}
			}
			else
			{
				int num2 = (int)_slotID;
				if (num2 > 4)
				{
					num2 -= 4;
				}
				text = string.Format("QuickSlot{0}", num2);
			}
			return text;
		}
		return string.Empty;
	}

	// Token: 0x0600233D RID: 9021 RVA: 0x000CD810 File Offset: 0x000CBA10
	public static string GetVirtualCursorClickName(int _mouseButtonID)
	{
		if (_mouseButtonID == 0)
		{
			return "VC_LeftClick";
		}
		if (_mouseButtonID == 1)
		{
			return "VC_RightClick";
		}
		if (_mouseButtonID == 2)
		{
			return "VC_MiddleClick";
		}
		return string.Empty;
	}

	// Token: 0x0600233E RID: 9022 RVA: 0x000CD840 File Offset: 0x000CBA40
	public static bool IsLastActionGamepad(int _playerID)
	{
		float lastTimeMouseMovement = ControlsInput.m_playerInputManager[_playerID].GetLastTimeMouseMovement();
		float lastTimeGamepadMovement = ControlsInput.m_playerInputManager[_playerID].GetLastTimeGamepadMovement();
		float lastTimeKeyboardMovement = ControlsInput.m_playerInputManager[_playerID].GetLastTimeKeyboardMovement();
		return lastTimeGamepadMovement > lastTimeMouseMovement && lastTimeGamepadMovement > lastTimeKeyboardMovement;
	}

	// Token: 0x0600233F RID: 9023 RVA: 0x000CD890 File Offset: 0x000CBA90
	public static bool IsKeyboardOrGamepadNavigation(int _playerID)
	{
		if (_playerID == -1)
		{
			_playerID = 0;
		}
		float lastTimeMouseMovement = ControlsInput.m_playerInputManager[_playerID].GetLastTimeMouseMovement();
		float lastTimeGamepadMovement = ControlsInput.m_playerInputManager[_playerID].GetLastTimeGamepadMovement();
		float lastTimeKeyboardMovement = ControlsInput.m_playerInputManager[_playerID].GetLastTimeKeyboardMovement();
		return lastTimeGamepadMovement > lastTimeMouseMovement || lastTimeKeyboardMovement > lastTimeMouseMovement;
	}

	// Token: 0x06002340 RID: 9024 RVA: 0x000CD8E8 File Offset: 0x000CBAE8
	public static string GetFirstElementMapNameWithGameplayAction(int _playerID, ControlsInput.GameplayActions _action, bool _ignoreJoystick = false, bool _ignoreMouse = false)
	{
		return ControlsInput.m_playerInputManager[_playerID].GetLastUsedControllerFirstElementMapNameWithAction(ControlsInput.GetGameplayActionName(_action), _ignoreJoystick, _ignoreMouse);
	}

	// Token: 0x06002341 RID: 9025 RVA: 0x000CD904 File Offset: 0x000CBB04
	public static GlyphData GetFirstElementMapWithGameplayAction(int _playerID, ControlsInput.GameplayActions _action, bool _ignoreJoystick = false, bool _ignoreMouse = false)
	{
		return ControlsInput.m_playerInputManager[_playerID].GetLastUsedControllerFirstElementMapWithAction(ControlsInput.GetGameplayActionName(_action), _ignoreJoystick, _ignoreMouse);
	}

	// Token: 0x06002342 RID: 9026 RVA: 0x000CD920 File Offset: 0x000CBB20
	public static GlyphData GetFirstElementMapWithMenuAction(int _playerID, ControlsInput.MenuActions _action, bool _ignoreJoystick, bool _ignoreMouse)
	{
		return ControlsInput.m_playerInputManager[_playerID].GetLastUsedControllerFirstElementMapWithAction(ControlsInput.GetMenuActionName(_action), _ignoreJoystick, _ignoreMouse);
	}

	// Token: 0x06002343 RID: 9027 RVA: 0x000CD93C File Offset: 0x000CBB3C
	public static GlyphData GetFirstElementMapWithVirtualCursorAction(int _playerID, int _mouseButtonID)
	{
		return ControlsInput.m_playerInputManager[_playerID].GetLastUsedControllerFirstElementMapWithAction(ControlsInput.GetVirtualCursorClickName(_mouseButtonID), false, false);
	}

	// Token: 0x06002344 RID: 9028 RVA: 0x000CD958 File Offset: 0x000CBB58
	public static string GetFirstElementMapNameWithQuickSlot(int _playerID, QuickSlot.QuickSlotIDs _slotID, bool _gamepad)
	{
		string quickSlotName = ControlsInput.GetQuickSlotName(_slotID, _gamepad);
		if (!string.IsNullOrEmpty(quickSlotName))
		{
			return ControlsInput.m_playerInputManager[_playerID].GetLastUsedControllerFirstElementMapNameWithAction(quickSlotName, false, false);
		}
		return string.Empty;
	}

	// Token: 0x06002345 RID: 9029 RVA: 0x000CD994 File Offset: 0x000CBB94
	public static GlyphData GetFirstElementMapWithQuickSlot(int _playerID, QuickSlot.QuickSlotIDs _slotID, bool _gamepad)
	{
		string quickSlotName = ControlsInput.GetQuickSlotName(_slotID, _gamepad);
		if (!string.IsNullOrEmpty(quickSlotName))
		{
			return ControlsInput.m_playerInputManager[_playerID].GetLastUsedControllerFirstElementMapWithAction(quickSlotName, false, false);
		}
		return default(GlyphData);
	}

	// Token: 0x06002346 RID: 9030 RVA: 0x000CD9D4 File Offset: 0x000CBBD4
	public static string GetFirstElementMap(int _playerID, string _action, bool _ignoreJoystick, bool _ignoreMouse = false)
	{
		return ControlsInput.m_playerInputManager[_playerID].GetLastUsedControllerFirstElementMapNameWithAction(_action, _ignoreJoystick, _ignoreMouse);
	}

	// Token: 0x06002347 RID: 9031 RVA: 0x000CD9EC File Offset: 0x000CBBEC
	// Note: this type is marked as 'beforefieldinit'.
	static ControlsInput()
	{
	}

	// Token: 0x040022AC RID: 8876
	private static readonly string[] REWIRED_GAMEPLAY_ACTION_NAMES = new string[]
	{
		"MoveHorizontal",
		"MoveVertical",
		"CameraMoveHorizontal",
		"CameraMoveVertical",
		"ToggleInventory",
		"Interact",
		"Attack1",
		"Attack2",
		"Block",
		"Sheathe",
		"Dodge",
		"Stealth",
		"Sprint",
		"LockHold",
		"LockToggle",
		"SwitchTargetHorizontal",
		"SwitchTargetVertical",
		"ConfirmDeploy",
		"CancelDeploy",
		"ChargeWeapon",
		"CancelCharge",
		"Aim",
		"DragCorpse",
		"ToggleLights",
		"HandleBag",
		"AutoRun"
	};

	// Token: 0x040022AD RID: 8877
	private static readonly string[] REWIRED_MENU_ACTION_NAMES = new string[]
	{
		"MenuVertical",
		"MenuUp",
		"MenuDown",
		"MenuHorizontal",
		"MenuLeft",
		"MenuRight",
		"QuickAction",
		"ShowOptions",
		"Cancel",
		"ToggleInventory",
		"ToggleEquipment",
		"ExitContainer",
		"TakeAll",
		"Info",
		"Help",
		"ToggleSkillMenu",
		"ToggleMap",
		"ToggleQuestLog",
		"ToggleStatus",
		"ToggleCrafting",
		"ToggleQuickSlot",
		"ToggleChat",
		"NextMenu",
		"PreviousMenu",
		"QuickDialogue",
		"Delete",
		"Compare",
		"PreviousFilter",
		"NextFilter",
		"ToggleEffects"
	};

	// Token: 0x040022AE RID: 8878
	private static readonly ControlsInput.GameplayActions[] m_specialListenedActions = new ControlsInput.GameplayActions[0];

	// Token: 0x040022AF RID: 8879
	private static readonly ControlsInput.GameplayActions[] m_quickReleaseActions = new ControlsInput.GameplayActions[]
	{
		ControlsInput.GameplayActions.Dodge,
		ControlsInput.GameplayActions.Stealth
	};

	// Token: 0x040022B0 RID: 8880
	private static readonly int[] m_startingRewiredPlayers = new int[]
	{
		0,
		1,
		2,
		3
	};

	// Token: 0x040022B1 RID: 8881
	private static List<int> m_availableRewiredPlayers;

	// Token: 0x040022B2 RID: 8882
	private static List<int> m_assignedRewiredPlayers = new List<int>();

	// Token: 0x040022B3 RID: 8883
	private static Dictionary<int, RewiredInputs> m_playerInputManager = new Dictionary<int, RewiredInputs>();

	// Token: 0x040022B4 RID: 8884
	private static RewiredInputs m_inputManager;

	// Token: 0x020004BB RID: 1211
	public enum MapCategories
	{
		// Token: 0x040022B6 RID: 8886
		Gameplay,
		// Token: 0x040022B7 RID: 8887
		Menu
	}

	// Token: 0x020004BC RID: 1212
	public enum GameplayActions
	{
		// Token: 0x040022B9 RID: 8889
		MoveHorizontal,
		// Token: 0x040022BA RID: 8890
		MoveVertical,
		// Token: 0x040022BB RID: 8891
		RotateCameraHorizontal,
		// Token: 0x040022BC RID: 8892
		RotateCameraVertical,
		// Token: 0x040022BD RID: 8893
		ToggleInventory,
		// Token: 0x040022BE RID: 8894
		Interact,
		// Token: 0x040022BF RID: 8895
		Attack1,
		// Token: 0x040022C0 RID: 8896
		Attack2,
		// Token: 0x040022C1 RID: 8897
		Block,
		// Token: 0x040022C2 RID: 8898
		Sheathe,
		// Token: 0x040022C3 RID: 8899
		Dodge,
		// Token: 0x040022C4 RID: 8900
		Stealth,
		// Token: 0x040022C5 RID: 8901
		Sprint,
		// Token: 0x040022C6 RID: 8902
		LockHold,
		// Token: 0x040022C7 RID: 8903
		LockToggle,
		// Token: 0x040022C8 RID: 8904
		SwitchTargetHorizontal,
		// Token: 0x040022C9 RID: 8905
		SwitchTargetVertical,
		// Token: 0x040022CA RID: 8906
		ConfirmDeploy,
		// Token: 0x040022CB RID: 8907
		CancelDeploy,
		// Token: 0x040022CC RID: 8908
		ChargeWeapon,
		// Token: 0x040022CD RID: 8909
		CancelCharge,
		// Token: 0x040022CE RID: 8910
		Aim,
		// Token: 0x040022CF RID: 8911
		DragCorpse,
		// Token: 0x040022D0 RID: 8912
		ToggleLights,
		// Token: 0x040022D1 RID: 8913
		HandleBag,
		// Token: 0x040022D2 RID: 8914
		AutoRun,
		// Token: 0x040022D3 RID: 8915
		Count
	}

	// Token: 0x020004BD RID: 1213
	public enum MenuActions
	{
		// Token: 0x040022D5 RID: 8917
		MoveVertical,
		// Token: 0x040022D6 RID: 8918
		MoveUp,
		// Token: 0x040022D7 RID: 8919
		MoveDown,
		// Token: 0x040022D8 RID: 8920
		MoveHorizontal,
		// Token: 0x040022D9 RID: 8921
		MoveLeft,
		// Token: 0x040022DA RID: 8922
		MoveRight,
		// Token: 0x040022DB RID: 8923
		QuickAction,
		// Token: 0x040022DC RID: 8924
		ShowOptions,
		// Token: 0x040022DD RID: 8925
		Cancel,
		// Token: 0x040022DE RID: 8926
		ToggleInventory,
		// Token: 0x040022DF RID: 8927
		ToggleEquipment,
		// Token: 0x040022E0 RID: 8928
		ExitContainer,
		// Token: 0x040022E1 RID: 8929
		TakeAll,
		// Token: 0x040022E2 RID: 8930
		InfoInput,
		// Token: 0x040022E3 RID: 8931
		ToggleHelp,
		// Token: 0x040022E4 RID: 8932
		ToggleSkillMenu,
		// Token: 0x040022E5 RID: 8933
		ToggleMapMenu,
		// Token: 0x040022E6 RID: 8934
		ToggleQuestMenu,
		// Token: 0x040022E7 RID: 8935
		ToggleCharacterStatus,
		// Token: 0x040022E8 RID: 8936
		ToggleCraftingMenu,
		// Token: 0x040022E9 RID: 8937
		ToggleQuickSlotMenu,
		// Token: 0x040022EA RID: 8938
		ToggleChatMenu,
		// Token: 0x040022EB RID: 8939
		GoToNextMenu,
		// Token: 0x040022EC RID: 8940
		GoToPreviousMenu,
		// Token: 0x040022ED RID: 8941
		QuickDialogue,
		// Token: 0x040022EE RID: 8942
		Delete,
		// Token: 0x040022EF RID: 8943
		Compare,
		// Token: 0x040022F0 RID: 8944
		PreviousFilter,
		// Token: 0x040022F1 RID: 8945
		NextFilter,
		// Token: 0x040022F2 RID: 8946
		ToggleEffectMenu
	}
	
	// Our custom function to map an input to a quickslot
	public static bool QuickSlotInstantX(int _playerID, int index)
	{
		return ControlsInput.m_playerInputManager[_playerID].GetButtonDown(string.Format("QS_Instant{0}", index));
	}
}
