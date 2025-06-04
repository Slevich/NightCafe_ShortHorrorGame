using TMPro;
using UnityEngine;
using Zenject;

public class InteractionTipManager : MonoBehaviour
{
    #region Fields
    [Header(""), SerializeField] private ScaleAnimation _textScaleAnimation;
    [Header("TextMeshPro UI with button name."), SerializeField] private TextMeshProUGUI _buttonNameText;
    [Header("TextMeshPro UI with interaction type."), SerializeField] private TextMeshProUGUI _interactionTypeText;
    [Header("TextMeshPro UI with item name."), SerializeField] private TextMeshProUGUI _itemNameText;
    private ItemsSelector _itemsSelector;
    private bool _tipShowed = false;
    #endregion

    #region Methods
    [Inject]
    public void Construct(ItemsSelector Selector)
    {
        _itemsSelector = Selector;

        if (_itemsSelector != null)
            _itemsSelector.OnSelectionCallback += (selectable) => TipManagement(selectable);
    }

    private void TipManagement(GameObject Selectable)
    {
        bool selectableExists = Selectable != null;

        if(selectableExists /*&& !_tipShowed*/)
        {
            _tipShowed = true;
            _textScaleAnimation.ScaleToEnd();
            InteractionTip tip = ((InteractionTip)ComponentsSearcher.GetSingleComponentOfTypeFromObjectAndChildren(Selectable, typeof(InteractionTip)));

            if(tip != null)
            {
                if (_buttonNameText != null)
                    _buttonNameText.text = tip.ButtonName;

                if (_interactionTypeText != null)
                    _interactionTypeText.text = tip.InteractionType;

                if (_itemNameText != null)
                    _itemNameText.text = tip.ItemName;
            }
        }
        else if(!selectableExists /*&& _tipShowed*/)
        {
            _tipShowed = false;
            _textScaleAnimation.ScaleToStart();
        }
    }
    #endregion
}
