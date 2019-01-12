using UnityEngine;
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

	Vector2 scrollPosition;

	[SerializeField]
	bool showGUIDComponent;

	void OnGUI()
	{
		using (new EditorGUILayout.VerticalScope()) {
			using (var changed = new EditorGUI.ChangeCheckScope()) {
				showGUIDComponent = EditorGUILayout.Toggle("Show In Inspector", showGUIDComponent);

				if (!!changed.changed) {
					EditorPrefs.SetBool("GUIDCollecterWindow.showGUIDComponent", showGUIDComponent);
					foreach (var item in FindObjectsOfType<GUIDComponent>()) {
						if (!!showGUIDComponent) {
							item.hideFlags = HideFlags.None;
						} else {
							item.hideFlags = HideFlags.HideInInspector;
						}

						EditorUtility.SetDirty(item);
					} 
				}
			}

			EditorGUILayout.Separator();

			using (var scroll = new EditorGUILayout.ScrollViewScope(scrollPosition, false, false)) {
				foreach (var item in GUIDCollector.Instance.allReference) {
					using (new EditorGUI.DisabledScope(true)) {
						using (new EditorGUILayout.HorizontalScope()) {
							EditorGUILayout.ObjectField(item.Value, item.Value.GetType(), true);
							EditorGUILayout.TextField(item.Key.ToString());
						}
					}
				}

				scrollPosition = scroll.scrollPosition;
			}
		}
	}

	void OnEnable()
	{
		GUIDCollector.Instance.onRegisterGUID += repaint;
		GUIDCollector.Instance.onUnregisterGUID += repaint;

		showGUIDComponent = EditorPrefs.GetBool("GUIDCollecterWindow.showGUIDComponent", true);
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
