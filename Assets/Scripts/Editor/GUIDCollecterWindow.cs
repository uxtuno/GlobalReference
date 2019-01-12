using UnityEditor;

/// <summary>
/// GUID が関連付けられたオブジェクトを表示するウインドウ
/// </summary>
public class GUIDCollecterWindow : EditorWindow
{
	[MenuItem("Window/GUID Collecter")]
	static void createWindow()
	{
		GUIDCollecterWindow window = GetWindow<GUIDCollecterWindow>();
		window.Show();
	}

	void OnGUI()
	{
		foreach (var item in GUIDCollector.Instance.allReference) {
			using (var scope1 = new EditorGUI.DisabledScope(true)) {
				using (var scope2 = new EditorGUILayout.HorizontalScope()) {
					EditorGUILayout.ObjectField(item.Value, item.Value.GetType(), true);
					EditorGUILayout.TextField(item.Key.ToString());
				}
			}
		}
	}

	void OnEnable()
	{
		GUIDCollector.Instance.onRegisterGUID += repaint;
		GUIDCollector.Instance.onUnregisterGUID += repaint;
	}

	void OnDisable()
	{
		GUIDCollector.Instance.onRegisterGUID -= repaint;
		GUIDCollector.Instance.onUnregisterGUID -= repaint;
	}

	void repaint(System.Guid guid)
	{
		Repaint();
	}
}
