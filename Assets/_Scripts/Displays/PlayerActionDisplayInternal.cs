using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class PlayerActionDisplayInternal : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;
    public TextMeshProUGUI Text => _text;
    [SerializeField] private GameObject _content;
    public GameObject Content => _content;
    
}
