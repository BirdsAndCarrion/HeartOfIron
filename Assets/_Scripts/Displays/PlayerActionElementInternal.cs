using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class PlayerActionElementInternal : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _actionText;
    public TextMeshProUGUI ActionText => _actionText;
    [SerializeField] private DragDrop _dragDrop;
    public DragDrop DragDrop => _dragDrop;
    [SerializeField] private Image _actionImage;
    public Image ActionImage => _actionImage;
    [SerializeField] private Button _actionButton;
    public Button ActionButton => _actionButton;
    [SerializeField] private Button _removeButton;
    public Button RemoveButton => _removeButton;
}
