using UnityEditor; // Import namespace chứa các lớp hỗ trợ xây dựng custom editor cho Unity
using UnityEngine; // Import namespace chứa các lớp cơ bản của Unity (GameObject, MonoBehaviour, etc.)

// Đánh dấu lớp này là một custom editor cho kiểu QuestManager
[CustomEditor(typeof(QuestManager))]
public class QuestDataDrawer : Editor
{
    // Biến để lưu trạng thái mở/đóng của dropdown
    private bool showQuestData = true;

    // Ghi đè phương thức OnInspectorGUI để tùy chỉnh cách hiển thị Inspector của QuestManager
    public override void OnInspectorGUI()
    {
        // Vẽ các trường mặc định của QuestManager (như các trường [SerializeField])
        DrawDefaultInspector();

        // Ép kiểu target (đối tượng đang được chỉnh sửa) về kiểu QuestManager
        QuestManager manager = (QuestManager)target;

        // Kiểm tra xem danh sách questList có khác null hay không
        if (manager.questList != null)
        {
            // Thêm khoảng cách giữa các phần hiển thị trong Inspector
            EditorGUILayout.Space();

            // Tạo dropdown với label "Chi tiết Quest Data" và cập nhật trạng thái mở/đóng
            showQuestData = EditorGUILayout.Foldout(showQuestData, "Chi tiết Quest Data", true);

            // Nếu dropdown đang được mở, hiển thị nội dung chi tiết của Quest Data
            if (showQuestData)
            {
                // Duyệt qua từng phần tử quest trong danh sách questList
                foreach (var quest in manager.questList)
                {
                    // Kiểm tra xem quest có khác null hay không
                    if (quest != null)
                    {
                        // Tạo một custom editor cho từng ScriptableObject (QuestData) được chứa trong danh sách
                        Editor questEditor = CreateEditor(quest);
                        // Vẽ giao diện Inspector cho ScriptableObject đó, hiển thị các thuộc tính của nó
                        questEditor.OnInspectorGUI();
                        // Thêm khoảng cách sau khi hiển thị từng quest để dễ phân biệt
                        EditorGUILayout.Space();
                    }
                }
            }
        }
    }
}
