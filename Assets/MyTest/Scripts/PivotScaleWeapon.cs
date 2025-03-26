using System.Collections;
using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;

public class WeaponAdjuster : MonoBehaviour
{
    // Định nghĩa offset ban đầu từ pivot (điểm cầm) theo không gian local của đối tượng con
    public Vector3 handleLocalOffset = new(0f, -0.5f, 0f);

    private Vector3 initialScale;
    private int Count = 1;

    private float timer = 0f;
    private bool isScaling = false;
    private float duration = 1f; // 1 giây

    private int level1 = 2;
    private int level2 = 3;
    private int level3 = 4;

    private Coroutine scalingCoroutine;

    [SerializeField] private LEVEL currentLevel;

    //private int currentLevel = 0; // 0: chưa scale, 1: đã đạt level1, 2: đã đạt level2, 3: đã đạt level3

    private enum LEVEL
    {
        DEFAULT,
        LEVEL0,
        LEVEL1,
        LEVEL2,
        LEVEL3,
    }    

    private void Start()
    {
        // Lưu lại scale ban đầu
        initialScale = transform.localScale;
    }

    // Hàm điều chỉnh scale và bù trừ vị trí
    public void ScaleAndAdjust(Vector3 newScale)
    {

        // Lưu vị trí world hiện tại của điểm cầm (có tính rotation hiện tại)
        Vector3 currentHandleWorldPos = transform.TransformPoint(handleLocalOffset); // worldPos = position + rotation × (localOffset × scale)

        // Áp dụng scale mới
        transform.localScale = newScale;

        // Sau khi scale, tính vị trí world mới của điểm cầm
        Vector3 newHandleWorldPos = transform.TransformPoint(handleLocalOffset);

        // worldPos = position + rotation × (localOffset × scale)
        //Có nghĩa là: chuyển đổi localOffset (đã nhân với scale)
        //từ không gian local sang không gian world bằng cách áp dụng rotation,
        //sau đó cộng với vị trí world của đối tượng để có được vị trí world thực sự của điểm đó.

        // Tính delta hiệu chỉnh
        Vector3 delta = currentHandleWorldPos - newHandleWorldPos;

        // Điều chỉnh lại vị trí của đối tượng để điểm cầm không dịch chuyển
        transform.position += delta;
    }

    private void Update()
    {
        //// Ví dụ: tăng giảm scale theo thời gian sử dụng PingPong
        //float scaleFactor = 1f + Mathf.PingPong(Time.time * 0.5f, 1f);
        //ScaleAndAdjust(initialScale * scaleFactor);

        //ScaleWhenPressButton();
        //ScaleWithTimer();

        if (Input.GetKeyDown(KeyCode.N))
        {
            // Reset và bắt đầu coroutine
            ResetScale();
            scalingCoroutine = StartCoroutine(ScaleCoroutine());
        }

        if (Input.GetKeyUp(KeyCode.M))
        {
            if (scalingCoroutine != null)
            {
                StopCoroutine(scalingCoroutine);
                scalingCoroutine = null;
            }
            ResetScale();
        }
    }

    private void ScaleWithTimer()
    {
        // Khi nhấn phím N, reset timer và currentLevel
        if (Input.GetKeyDown(KeyCode.N))
        {
            timer = 0f;
            isScaling = true;
            currentLevel = LEVEL.LEVEL0;
        }

        if (isScaling && Input.GetKey(KeyCode.N))
        {
            timer += Time.deltaTime;

            // Khi vượt qua 1*duration và chưa đạt LEVEL1
            if (timer >= duration && currentLevel == LEVEL.LEVEL0)
            {
                Debug.Log("lv1");
                ScaleAndAdjust(initialScale * level1);
                currentLevel = LEVEL.LEVEL1;
            }
            // Khi vượt qua 2*duration và chưa đạt LEVEL2
            else if (timer >= 2 * duration && currentLevel == LEVEL.LEVEL1)
            {
                Debug.Log("lv2");
                ScaleAndAdjust(initialScale * level2);
                currentLevel = LEVEL.LEVEL2;
            }
            // Khi vượt qua 3*duration và chưa đạt LEVEL3
            else if (timer >= 3 * duration && currentLevel == LEVEL.LEVEL2)
            {
                Debug.Log("lv3");
                ScaleAndAdjust(initialScale * level3);
                currentLevel = LEVEL.LEVEL3;
                // Nếu muốn dừng ngay khi đạt LEVEL3
                isScaling = false;
            }
        }

        // Khi thả phím N, reset trạng thái nếu cần
        if (Input.GetKeyUp(KeyCode.M))
        {
            isScaling = false;
            timer = 0f;
            currentLevel = LEVEL.LEVEL0;
            ScaleAndAdjust(initialScale * 1);
        }
    }

    private void ScaleWhenPressButton()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            Count++;
            Vector3 newScale = initialScale * Count;
            ScaleAndAdjust(newScale);
        }
    }

    private IEnumerator ScaleCoroutine()
    {
        float timer = 0f;
        while (true)
        {
            timer += Time.deltaTime;

            if (timer >= 3 * duration && currentLevel == LEVEL.LEVEL2)
            {
                Debug.Log("lv3");
                ScaleAndAdjust(initialScale * level3);
                currentLevel = LEVEL.LEVEL3;
                yield break; // Dừng coroutine khi đạt level3
            }
            else if (timer >= 2 * duration && currentLevel == LEVEL.LEVEL1)
            {
                Debug.Log("lv2");
                ScaleAndAdjust(initialScale * level2);
                currentLevel = LEVEL.LEVEL2;
            }
            else if (timer >= duration && currentLevel == LEVEL.LEVEL0)
            {
                Debug.Log("lv1");
                ScaleAndAdjust(initialScale * level1);
                currentLevel = LEVEL.LEVEL1;
            }
            yield return null;
        }
    }

    private void ResetScale()
    {
        currentLevel = LEVEL.LEVEL0;
    }
}
