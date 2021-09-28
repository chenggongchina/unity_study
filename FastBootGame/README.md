可以在unity客户端面板的播放按键左侧增加一个"P"键，用于快速启动入口场景。

在Editor/SceneSwitcher.cs的OnToolbarGUI方法中进行配置。


样例:

```
		static void OnToolbarGUI()
		{
			GUILayout.FlexibleSpace();

			if(GUILayout.Button(new GUIContent("P", "Start Play"), ToolbarStyles.commandButtonStyle))
			{
				SceneHelper.StartScene("Assets/Scenes/LoginScene.unity");
            }
        }
```