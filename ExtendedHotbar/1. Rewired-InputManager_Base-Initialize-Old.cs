using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using Rewired.Config;
using Rewired.Data;
using Rewired.Internal;
using Rewired.Platforms;
using Rewired.Utils;
using Rewired.Utils.Interfaces;
using UnityEngine;

namespace Rewired
{
	// Token: 0x020002B4 RID: 692
	[EditorBrowsable(EditorBrowsableState.Never)]
	[Browsable(false)]
	[AddComponentMenu("")]
	[ExecuteInEditMode]
	public abstract partial class InputManager_Base : MonoBehaviour
	{
		// Token: 0x06001688 RID: 5768 RVA: 0x000931E8 File Offset: 0x000913E8
		private void Initialize()
		{
			if (this._duplicateRIMError)
			{
				return;
			}
			try
			{
				if (!this.IsOnlyManagerInScene())
				{
					goto IL_14;
				}
				goto IL_223;
				int num;
				for (;;)
				{
					IL_19:
					string text;
					switch (num ^ 1426088357)
					{
					case 0:
						goto IL_223;
					case 1:
						goto IL_DA;
					case 2:
						goto IL_97;
					case 3:
						if (!string.IsNullOrEmpty(text))
						{
							Logger.LogWarning(text);
							num = 1426088360;
							continue;
						}
						break;
					case 4:
						if (base.gameObject.GetComponent<OnGUIHelper>() == null)
						{
							base.gameObject.AddComponent<OnGUIHelper>();
							num = 1426088354;
							continue;
						}
						goto IL_16D;
					case 5:
						if ((this._userData.ConfigVars.updateLoop & UpdateLoopSetting.Update) == UpdateLoopSetting.None)
						{
							this.userData.ConfigVars.updateLoop |= UpdateLoopSetting.Update;
							num = 1426088363;
							continue;
						}
						goto IL_1FA;
					case 6:
						goto IL_112;
					case 7:
						goto IL_16D;
					case 8:
						UnityTools.qsVUUwnbwVBaeFiTzevBGdvpYDuH(this.platform, this.editorPlatform, this.isEditor, this.webplayerPlatform, this.GetExternalTools());
						ReInput.qsVUUwnbwVBaeFiTzevBGdvpYDuH(this, new Func<ConfigVars, object>(this.InitializePlatform), this._userData.ConfigVars, this._controllerDataFiles, this._userData);
						this.initialized = true;
						this.criticalError = false;
						num = 1426088358;
						continue;
					case 9:
						goto IL_14;
					case 10:
						if (Application.isPlaying)
						{
							UnityEngine.Object.DontDestroyOnLoad(base.transform.root.gameObject);
							num = 1426088355;
							continue;
						}
						goto IL_112;
					case 11:
						if (this._userData.ConfigVars != null)
						{
							num = ((this._controllerDataFiles == null) ? 1426088359 : 1426088352);
							continue;
						}
						goto IL_97;
					case 12:
						goto IL_65;
					case 14:
						goto IL_1FA;
					}
					break;
					IL_97:
					Logger.LogError("Error! DataFiles is missing or corrupt! Make sure you have the DataFiles file linked in the inspector.");
					num = 1426088361;
					continue;
					IL_112:
					this.DetectPlatform();
					num = ((this._userData == null) ? 1426088359 : 1426088366);
					continue;
					IL_16D:
					text = this.SetPlatformToEditorPlatform();
					num = 1426088365;
					continue;
					IL_1FA:
					num = (((this._userData.ConfigVars.updateLoop & UpdateLoopSetting.OnGUI) == UpdateLoopSetting.OnGUI) ? 1426088353 : 1426088354);
				}
				goto IL_23F;
				IL_65:
				IL_DA:
				return;
				IL_23F:
				this.OnInitialized();
				return;
				IL_14:
				num = 1426088356;
				goto IL_19;
				IL_223:
				num = ((!this._dontDestroyOnLoad) ? 1426088355 : 1426088367);
				goto IL_19;
			}
			catch (Exception exception)
			{
				this.HandleException(InputManager_Base.ExceptionPoint.Initialization, "initialization", exception);
			}
		}
	}
}