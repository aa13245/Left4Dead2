using UnityEngine;

public class LightController : MonoBehaviour, InteractObj_JSW
{
    LevelDesign levelDesign;
    // 무대 조명 On 상호작용
    public bool Interact()
    {
        if (levelDesign.LightOn()) return true;
        else return false;
    }

    // Start is called before the first frame update
    void Start()
    {
        levelDesign = GameObject.Find("LevelDesign").GetComponent<LevelDesign>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
