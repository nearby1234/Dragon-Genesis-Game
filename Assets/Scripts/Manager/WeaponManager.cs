using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public Transform rightHand;  // Transform của tay phải của nhân vật
    public Transform weaponParent; // Đối tượng cha chứa vũ khí
    [SerializeField] private Transform currentWeapon; // Vũ khí hiện tại


    private void Start()
    {
       
    }
    // Hàm thay đổi vũ khí
    public void ChangeWeapon(Transform newWeapon)
    {
        
        // Nếu có vũ khí cũ, hủy bỏ nó
        if (currentWeapon != null)
        {
            Destroy(currentWeapon.gameObject);
        }

        // Gắn vũ khí mới vào đúng vị trí của tay phải
        currentWeapon = Instantiate(newWeapon, rightHand.position, rightHand.rotation, weaponParent);

        // Điều chỉnh lại vị trí và góc độ local của vũ khí để khớp với tay phải
        AdjustWeaponPosition(currentWeapon);
        //AdjustWeaponScale(currentWeapon);
    }

    private void AdjustWeaponPosition(Transform weapon)
    {
        
        // Đặt vũ khí đúng vị trí tay phải, tính theo local position
        weapon.localPosition = Vector3.zero; // Có thể điều chỉnh lại nếu cần
        weapon.localRotation = Quaternion.identity; // Tương tự, có thể thay đổi nếu cần
    }

    //private void AdjustWeaponScale(Transform weapon)
    //{
    //    // Giả sử bạn có một giá trị tham chiếu cho kích thước tay phải của nhân vật
    //    float scaleFactor = CalculateScaleFactor(weapon);

    //    // Điều chỉnh lại tỷ lệ vũ khí sao cho phù hợp
    //    weapon.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);
    //}

    //private float CalculateScaleFactor(Transform weapon)
    //{
    //    // Tính toán tỷ lệ dựa trên kích thước của tay phải và vũ khí
    //    float rightHandSize = rightHand.GetComponent<Renderer>().bounds.size.magnitude;
    //    float weaponSize = weapon.GetComponent<Renderer>().bounds.size.magnitude;

    //    return rightHandSize / weaponSize;
    //}
}
